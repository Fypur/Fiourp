using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Fiourp
{
    public abstract class Kinematic : Entity
    {
        public Vector2 Velocity;
        public AABBCollider Collider;

        protected float xRemainder;
        protected float yRemainder;

        public Vector2 ExactPos
        {
            get => new Vector2(Pos.X + xRemainder, Pos.Y + yRemainder);
            set
            {
                Pos = VectorHelper.Floor(value);
                xRemainder = value.X - (float)Math.Floor(value.X);
                yRemainder = value.Y - (float)Math.Floor(value.Y);
            }
        }

        public Kinematic(Vector2 position, AABBCollider collider, Sprite sprite) : base(position)
        {
            Collider = collider;

            AddComponent(collider);
            AddComponent(sprite);
        }

        public bool CollideAt(Kinematic collider, Vector2 position)
            => CollideAt(new List<Kinematic> { collider }, position, out _);

        public bool CollideAt(List<Kinematic> checkedColliders, Vector2 position)
            => CollideAt(checkedColliders, position, out _);

        public bool CollideAt(List<Kinematic> checkedKinematics, Vector2 position, out Kinematic collidedEntity)
        {
            Vector2 oldPos = Pos;
            Pos = position;

            Collider.Update();

            collidedEntity = null;

            foreach (Kinematic kinematic in checkedKinematics)
                if (Collider.Collide(kinematic.Collider) && kinematic != this)
                {
                    collidedEntity = kinematic;
                    break;
                }

            Pos = oldPos;
            Collider.Update();

            return collidedEntity != null;
        }
    }
}
