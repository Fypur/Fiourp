using Microsoft.Xna.Framework;
using System;

namespace Fiourp
{
    public class Rigidbody : Component
    {
        public Vector2 Position;
        public float Rotation;
        public Vector2 Velocity;
        public float AngularVelocity;

        public Vector2 Size;
        public float Friction;
        public float InvMass;
        public float InvI;

        public Vector2 Forces;
        public float Torque;
    }
}
