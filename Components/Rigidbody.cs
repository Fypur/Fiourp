using Microsoft.Xna.Framework;
using System;

namespace Fiourp
{
    public class Rigidbody : Component
    {
        //public Vector2 Position => ;
        //public float Rotation; Handled by Collider
        public Vector2 Velocity;
        public float AngularVelocity;

        //public Vector2 Size;
        public float Friction;
        public float InvMass;
        public float InvI;

        public Vector2 Forces;
        public float Torque;

        public Collider Collider => ParentEntity.Collider;

        public Rigidbody(float invMass, float invI)
        {
            InvMass = invMass;
            InvI = invI;
        }

        public override void Added()
        {
            Engine.CurrentMap.Data.Bodies.Add(this);
            base.Added();
        }

        public override void Removed()
        {
            if (Engine.CurrentMap.Data.Bodies.Contains(this))
                Engine.CurrentMap.Data.Bodies.Remove(this);
            base.Removed();
        }
    }
}
