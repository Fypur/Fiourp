using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public static class Physics
    {
        private static List<Contact> contacts = new();

        public static int Iterations = 10;
        public static float BiasImpulseBeta = 0.2f;

        public class Contact
        {
            public Rigidbody Reference;
            public Rigidbody Incident;

            public Vector2 Position;
            public Vector2 Normal;

            public float Pn;
            public float Pt;

            public float NormalMass;
            public float TangentialMass;

            private float friction;

            public Contact(Rigidbody reference, Rigidbody incident, Vector2 position, Vector2 normal)
            {
                Reference = reference;
                Incident = incident;
                Position = position;
                Normal = normal;
                friction = (float)Math.Sqrt(reference.Friction * incident.Friction);

                Debug.PointUpdate(position);
            }

            /// <summary>
            /// Calculate Effective masses
            /// </summary>
            public void PreStep()
            {
                Vector2 center1 = 0.5f * ((BoxColliderRotated)Reference.Collider).Rect[2] + 0.5f * ((BoxColliderRotated)Reference.Collider).Rect[0];
                Vector2 center2 = 0.5f * ((BoxColliderRotated)Incident.Collider).Rect[2] + 0.5f * ((BoxColliderRotated)Incident.Collider).Rect[0];
                Vector2 r1 = Position - center1;
                Vector2 r2 = Position - center2;

                Vector2 DoubleVectProd(Vector2 r, Vector2 n)
                    => new Vector2(-r.Y * (r.X * n.Y - r.Y * n.X), r.X * (r.X * n.Y - r.Y * n.X));

                Vector2 v = Reference.InvI * DoubleVectProd(r1, Normal) + Incident.InvI * DoubleVectProd(r2, Normal);
                NormalMass = 1 / (Reference.InvMass + Incident.InvMass + Vector2.Dot(v, Normal));

                Vector2 tangent = VectorHelper.Normal(Normal);
                TangentialMass = 1 / (Reference.InvMass + Incident.InvMass + Vector2.Dot(v, tangent));
            }

            public void ApplyImpulse()
            {
                //Give Contact IDs to every contact and create arbiters
                //TODO: Replace this with something more general, like rigidBody pivot center
                Vector2 center1 = 0.5f * ((BoxColliderRotated)Reference.Collider).Rect[2] + 0.5f * ((BoxColliderRotated)Reference.Collider).Rect[0];
                Vector2 center2 = 0.5f * ((BoxColliderRotated)Incident.Collider).Rect[2] + 0.5f * ((BoxColliderRotated)Incident.Collider).Rect[0];
                Vector2 r1 = Position - center1;
                Vector2 r2 = Position - center2;


                Vector2 dV = Incident.Velocity + Incident.AngularVelocity * new Vector2(-r2.Y, r2.X) - (Reference.Velocity + Reference.AngularVelocity * new Vector2(-r1.Y, r1.X)); //Vector Product translated in coords because not 3D lol

                float vn = Vector2.Dot(dV, Normal);
                Pn = Math.Max(-vn * NormalMass, 0);


                Vector2 tangent = VectorHelper.Normal(Normal);
                float vt = Vector2.Dot(dV, tangent);

                Pt = Math.Clamp(-vt * TangentialMass, -friction * Pn, friction * Pn);

                Vector2 P = Pn * Normal + Pt * tangent;
                Reference.Velocity -= P * Reference.InvMass;
                Reference.AngularVelocity -= Reference.InvI * (r1.X * P.Y - r1.Y * P.X);
                Incident.Velocity += P * Incident.InvMass;
                Incident.AngularVelocity += Incident.InvI * (r2.X * P.Y - r2.Y * P.X);

                //TODO: Angular velocity
                //TODO: Add bias impulse
            }
        }

        public class BoxContact
        {
            public bool Colliding;

            public Rigidbody Reference;
            public Rigidbody Incident;
            public Vector2 Normal;

            public Vector2 ReferenceFace1;
            public Vector2 ReferenceFace2;
            public Vector2 ClippedIncidentFace1;
            public Vector2 ClippedIncidentFace2;
        }

        public static List<Contact> SeparateBoxContacts(BoxContact boxContact)
        {
            bool found = false;
            List<Contact> contacts = new();
            if (boxContact.Reference.Collider.Collide(boxContact.ClippedIncidentFace1))
                contacts.Add(new Contact(boxContact.Reference.ParentEntity.GetComponent<Rigidbody>(), boxContact.Incident, boxContact.ClippedIncidentFace1, boxContact.Normal));
            if (boxContact.Reference.Collider.Collide(boxContact.ClippedIncidentFace2))
                contacts.Add(new Contact(boxContact.Reference.ParentEntity.GetComponent<Rigidbody>(), boxContact.Incident, boxContact.ClippedIncidentFace2, boxContact.Normal));
            return contacts;
        }

        public static void BroadPhase()
        {
            //TODO: Make this faster and possible for circle collders etc
            //TODO: Add arbiters for warm start (???)

            contacts.Clear(); //REMOVE THIS once arbiters are implemented

            for(int i = 0; i < Engine.CurrentMap.Data.Bodies.Count; i++)
            {
                for (int j = i + 1; j < Engine.CurrentMap.Data.Bodies.Count; j++)
                {
                    Rigidbody rb1 = Engine.CurrentMap.Data.Bodies[i];
                    Rigidbody rb2 = Engine.CurrentMap.Data.Bodies[j];

                    if (rb1.InvMass == 0 && rb2.InvMass == 0)
                        continue;

                    //TODO: Use arbiters from past frame to check if contact already existed before (face <-> face for example). If so, initialize impulse as the one from the last frame

                    BoxContact boxContact = Collision.BoxBoxClipping((BoxColliderRotated)rb1.Collider, (BoxColliderRotated)rb2.Collider);
                    if (boxContact.Colliding)
                        contacts.AddRange(SeparateBoxContacts(boxContact));
                }
            }

            Debug.LogUpdate(contacts.Count);
        }

        public static void SolveConstraints()
        {
            //Loop to solve all constraints
            for (int i = 0; i < 10; i++)
            {
                foreach (Contact contact in contacts)
                    contact.ApplyImpulse();
            }
        }

        public static void Update()
        {
            BroadPhase();

            //Apply Forces
            foreach (Rigidbody rb in Engine.CurrentMap.Data.Bodies)
            {
                rb.Velocity += rb.InvMass * rb.Forces * Engine.Deltatime;
                rb.AngularVelocity += rb.InvI * rb.Torque * Engine.Deltatime;
            }

            foreach(Contact contact in contacts)
                contact.PreStep();

            SolveConstraints();

            foreach (Rigidbody rb in Engine.CurrentMap.Data.Bodies)
            {
                rb.ParentEntity.Pos += rb.Velocity * Engine.Deltatime;
                rb.ParentEntity.Rotation += rb.AngularVelocity * Engine.Deltatime;

                //Putting it between -pi and +pi
                rb.ParentEntity.Rotation = rb.ParentEntity.Rotation - (float)Math.Floor(rb.ParentEntity.Rotation / (2 * float.Pi)) * 2f * float.Pi;
                if (rb.ParentEntity.Rotation > Math.PI) rb.ParentEntity.Rotation -= 2 * float.Pi;

                ((BoxColliderRotated)rb.ParentEntity.Collider).Rotation = rb.ParentEntity.Rotation; //TODO: Generalize

                rb.Forces = Vector2.Zero;
                rb.Torque = 0;
            }
        }
    }
}
