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
        private static List<Contact> contacts;

        public static float BiasImpulseBeta = 0.2f;

        public class Contact
        {
            public Rigidbody Reference;
            public Rigidbody Incident;

            public Vector2 Position;
            public Vector2 Normal;

            public Contact(Rigidbody reference, Rigidbody incident, Vector2 position, Vector2 normal)
            {
                Reference = reference;
                Incident = incident;
                Position = position;
                Normal = normal;
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

        public static void ApplyForces()
        {
            foreach(Rigidbody rb in Engine.CurrentMap.Data.Bodies)
            {
                rb.Velocity += rb.InvMass * rb.Forces * Engine.Deltatime;
                rb.AngularVelocity += rb.InvI * rb.Torque * Engine.Deltatime;
            }
        }

        public static void UpdatePositions()
        {
            foreach (Rigidbody rb in Engine.CurrentMap.Data.Bodies)
            {
                rb.ParentEntity.Pos += rb.Velocity * Engine.Deltatime;
                rb.ParentEntity.Rotation += rb.AngularVelocity * Engine.Deltatime;

                rb.Forces = Vector2.Zero;
                rb.Torque = 0;
            }
        }

        public static List<Contact> SeparateContacts(BoxContact boxContact)
        {
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
            foreach (Rigidbody rb1 in Engine.CurrentMap.Data.Bodies)
            {
                foreach (Rigidbody rb2 in Engine.CurrentMap.Data.Bodies)
                {
                    if (rb1.InvMass == 0 && rb2.InvMass == 0)
                        continue;
                    
                    if (rb1.Collider.Collide(rb2.Collider))
                        contacts.AddRange(SeparateContacts(Collision.BoxBoxClipping((BoxColliderRotated)rb1.Collider, (BoxColliderRotated)rb2.Collider)));
                }
            }
        }

        public static Vector2 ComputeImpulse(Contact contact)
        {
            //Give Contact IDs to every contact and create arbiters

            
            Vector2 center1 = 0.5f * ((BoxColliderRotated)contact.Reference.Collider).Rect[2] + 0.5f * ((BoxColliderRotated)contact.Reference.Collider).Rect[0]; //TODO: Replace this with something more general, like rigidBody pivot center
            Vector2 center2 = 0.5f * ((BoxColliderRotated)contact.Incident.Collider).Rect[2] + 0.5f * ((BoxColliderRotated)contact.Incident.Collider).Rect[0];

            Vector2 r1 = contact.Position - center1;
            Vector2 r2 = contact.Position - center2;

            Vector2 deltaV = contact.Reference.Velocity + contact.Reference.AngularVelocity * VectorHelper.Normal(r1) - (contact.Incident.Velocity + contact.Incident.AngularVelocity * VectorHelper.Normal(r2));

            Vector2 DoubleVectProd(Vector2 r, Vector2 n)
                => new Vector2(-r.Y * (r.X * n.Y - r.Y * n.X), r.X * (r.X * n.Y - r.Y * n.X));

            float effectiveMass = 1 / (contact.Reference.InvMass + contact.Incident.InvMass + Vector2.Dot(contact.Reference.InvI * DoubleVectProd(r1, contact.Normal) + contact.Incident.InvI * DoubleVectProd(r2, contact.Normal), contact.Normal));

            Vector2 Pn = -deltaV * effectiveMass;

            //Add bias impulse and tangential Impulse
            return Vector2.Zero;
        }

        public static void SolveConstraints()
        {
            //Loop to solve all constraints
            for (int i = 0; i < 10; i++)
            {
                //?????
            }
        }
    }
}
