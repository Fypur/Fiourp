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

        public Rigidbody(float mass, float I, float friction)
        {
            if(mass == float.PositiveInfinity)
                InvMass = 0;
            else
                InvMass = 1f/mass;
            if (I == float.PositiveInfinity)
                InvI = 0;
            else
                InvI = 1f/I;

                Friction = friction;
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
