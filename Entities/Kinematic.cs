using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Fiourp
{
    public abstract class Kinematic : Entity
    {
        public Vector2 Velocity;
        public Collider Collider;
        public Sprite Sprite;

        protected float xRemainder;
        protected float yRemainder;

        public Vector2 MiddlePos
        {
            get => new Vector2(Pos.X + Collider.Bounds.Width / 2, Pos.Y + Collider.Bounds.Height / 2);
            set
            {
                Vector2 pos = value - new Vector2(Collider.Bounds.Width / 2, Collider.Bounds.Height / 2);
                Pos = VectorHelper.Floor(pos);
            }
        }

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

        public Vector2 MiddleExactPos
        {
            get => new Vector2(Pos.X + xRemainder + Collider.Bounds.Width / 2, Pos.Y + yRemainder + Collider.Bounds.Height / 2);
            set
            {
                Vector2 pos = value - new Vector2(Collider.Bounds.Width / 2, Collider.Bounds.Height / 2);
                Pos = VectorHelper.Floor(pos);
                xRemainder = pos.X - (float)Math.Floor(pos.X);
                yRemainder = pos.Y - (float)Math.Floor(pos.Y);
            }
        }

        public Kinematic(Vector2 position, Collider collider, Sprite sprite) : base(position)
        {
            AddComponent(Collider = collider);

            if (sprite != null)
                AddComponent(Sprite = sprite);
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
