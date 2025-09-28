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
        public static float KBias = 0.2f;
        public static float SlopPenetration = 0.1f;

        public class Contact
        {
            public Rigidbody Reference;
            public Rigidbody Incident;

            private Vector2 position;
            private Vector2 normal;
            private float penetration;

            public float Pn; //accumulated impulses
            public float Pt;

            private float normalMass;
            private float tangentialMass;
            private float friction;


            public Contact(Rigidbody reference, Rigidbody incident, Vector2 position, Vector2 normal, float penetration)
            {
                Reference = reference;
                Incident = incident;
                this.position = position;
                this.normal = normal;
                this.penetration = penetration;
                friction = (float)Math.Sqrt(reference.Friction * incident.Friction);
            }

            /// <summary>
            /// Calculate Effective masses
            /// </summary>
            public void PreStep()
            {
                Vector2 center1 = 0.5f * ((BoxColliderRotated)Reference.Collider).Rect[2] + 0.5f * ((BoxColliderRotated)Reference.Collider).Rect[0];
                Vector2 center2 = 0.5f * ((BoxColliderRotated)Incident.Collider).Rect[2] + 0.5f * ((BoxColliderRotated)Incident.Collider).Rect[0];
                Vector2 r1 = position - center1;
                Vector2 r2 = position - center2;

                Vector2 DoubleVectProd(Vector2 r, Vector2 n)
                    => new Vector2(-r.Y * (r.X * n.Y - r.Y * n.X), r.X * (r.X * n.Y - r.Y * n.X));

                Vector2 v = Reference.InvI * DoubleVectProd(r1, normal) + Incident.InvI * DoubleVectProd(r2, normal);
                normalMass = 1 / (Reference.InvMass + Incident.InvMass + Vector2.Dot(v, normal));

                Vector2 tangent = VectorHelper.Normal(normal);
                tangentialMass = 1 / (Reference.InvMass + Incident.InvMass + Vector2.Dot(v, tangent));
            }

            public void ApplyImpulse()
            {
                //Give Contact IDs to every contact and create arbiters
                //TODO: Replace this with something more general, like rigidBody pivot center
                Vector2 center1 = 0.5f * ((BoxColliderRotated)Reference.Collider).Rect[2] + 0.5f * ((BoxColliderRotated)Reference.Collider).Rect[0];
                Vector2 center2 = 0.5f * ((BoxColliderRotated)Incident.Collider).Rect[2] + 0.5f * ((BoxColliderRotated)Incident.Collider).Rect[0];
                Vector2 r1 = position - center1;
                Vector2 r2 = position - center2;


                Vector2 dV = Incident.Velocity + Incident.AngularVelocity * new Vector2(-r2.Y, r2.X) - (Reference.Velocity + Reference.AngularVelocity * new Vector2(-r1.Y, r1.X)); //Vector Product translated in coords because not 3D lol

                float vn = Vector2.Dot(dV, normal);
                float vbias = KBias * (Engine.Deltatime == 0 ? 0 : 1 / Engine.Deltatime) * Math.Max(0, penetration - SlopPenetration);
                float pnt = Math.Max((-vn + vbias) * normalMass, 0);
                float tpp = pnt;

                float temp = Pn; //Accumulated impulse
                Pn = Math.Max(Pn + pnt, 0);
                pnt = Pn - temp;

                if (Math.Abs(tpp - pnt) >= 0.01f)
                    Debug.Log(pnt - tpp);

                Vector2 tangent = VectorHelper.Normal(normal);
                float vt = Vector2.Dot(dV, tangent);

                float ptt = Math.Clamp(-vt * tangentialMass, -friction * Pn, friction * Pn);
                temp = Pt;
                Pt = Math.Clamp(Pt + ptt, -friction * Pn, friction * Pn);
                ptt = Pt - temp;

                Vector2 P = pnt * normal + ptt * tangent;
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
            public float Penetration;

            public Vector2 ReferenceFace1;
            public Vector2 ReferenceFace2;
            public Vector2 ClippedIncidentFace1;
            public Vector2 ClippedIncidentFace2;
        }

        public static List<Contact> SeparateBoxContacts(BoxContact boxContact)
        {
            List<Contact> contacts = new();
            if (boxContact.Reference.Collider.Collide(boxContact.ClippedIncidentFace1))
                contacts.Add(new Contact(boxContact.Reference.ParentEntity.GetComponent<Rigidbody>(), boxContact.Incident, boxContact.ClippedIncidentFace1, boxContact.Normal, boxContact.Penetration));
            if (boxContact.Reference.Collider.Collide(boxContact.ClippedIncidentFace2))
                contacts.Add(new Contact(boxContact.Reference.ParentEntity.GetComponent<Rigidbody>(), boxContact.Incident, boxContact.ClippedIncidentFace2, boxContact.Normal, boxContact.Penetration));
            return contacts;
        }

        public static void BroadPhase()
        {
            //TODO: Use a dictionnary or sum, this is horribly slow
            //TODO: Make this faster and possible for circle collders etc

            List<Contact> newContacts = new();
            for(int i = 0; i < Engine.CurrentMap.Data.Bodies.Count; i++)
            {
                for (int j = i + 1; j < Engine.CurrentMap.Data.Bodies.Count; j++)
                {
                    Rigidbody rb1 = Engine.CurrentMap.Data.Bodies[i];
                    Rigidbody rb2 = Engine.CurrentMap.Data.Bodies[j];

                    if (rb1.InvMass == 0 && rb2.InvMass == 0)
                        continue;


                    BoxContact boxContact = Collision.BoxBoxClipping((BoxColliderRotated)rb1.Collider, (BoxColliderRotated)rb2.Collider);
                    if (boxContact.Colliding)
                    {
                        List<Contact> separate = SeparateBoxContacts(boxContact);

                        //TODO: check for edge ID
                        Contact c = contacts.Find((c2) => (c2.Reference == rb1 && c2.Incident == rb2) || (c2.Reference == rb1 && c2.Incident == rb2));
                        if(c != null)
                        {
                            separate[0].Pn = c.Pn; //Super ghetto
                            separate[0].Pt = c.Pt;
                        }

                        newContacts.AddRange(separate);
                    }
                }
            }

            contacts = newContacts;
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
