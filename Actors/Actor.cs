using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Fiourp
{
    /// <summary>
    /// Entity that moves and collides with things
    /// </summary>
    public abstract class Actor : Entity
    {
        public Vector2 Velocity;

        public float gravityScale;
        public static readonly Vector2 gravityVector = new Vector2(0, 9.81f);

        public Vector2 ExactPos => new Vector2(Pos.X + xRemainder, Pos.Y + yRemainder);
        private float xRemainder;
        private float yRemainder;


        public virtual bool IsRiding(Solid solid)
            => new Rectangle(Pos.ToPoint(), new Point(Width, Height + 1)).Intersects(new Rectangle(solid.Pos.ToPoint(), new Point(solid.Width, solid.Height)));

        public virtual void Squish() 
            => Engine.CurrentMap.Destroy(this);


        public Actor(Vector2 position, int width, int height, float gravityScale, Sprite sprite) 
            : base(position, width, height, sprite)
        {
            this.gravityScale = gravityScale;
        }

        public void MoveX(float amount, Action CallbackOnCollision = null)
        {
            xRemainder += amount;
            int move = (int)Math.Round(xRemainder);

            if(move != 0)
            {
                xRemainder -= move;
                int sign = Math.Sign(amount);

                while (move != 0)
                {
                    if (!Collider.CollideAt(Pos + new Vector2(sign, 0)))
                    {
                        Pos.X += sign;
                        move -= sign;
                    }
                    else
                    {
                        CallbackOnCollision?.Invoke();
                        break;
                    }
                }
            }
        }

        public void MoveY(float amount, Action CallbackOnSolidCollision = null)
        {
            yRemainder += amount;
            int move = (int)Math.Round(yRemainder);

            if (move != 0)
            {
                yRemainder -= move;
                int sign = Math.Sign(amount);

                while (move != 0)
                {
                    if (!Collider.CollideAt(Pos + new Vector2(0, sign)))
                    {
                        Pos.Y += sign;
                        move -= sign;
                    }
                    else
                    {
                        CallbackOnSolidCollision?.Invoke();
                        break;
                    }
                }
            }
        }

        public void MoveTo(Vector2 pos, Action CallbackOnCollisionX = null, Action CallbackOnCollisionY = null) 
        {
            MoveX(pos.X - Pos.X, CallbackOnCollisionX);
            MoveY(pos.Y - Pos.Y, CallbackOnCollisionY);
        }

        public void Gravity()
        {
            Velocity += gravityVector * gravityScale;
        }
    }
}