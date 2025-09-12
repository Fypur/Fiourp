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

        public static readonly Vector2 gravityVector = new Vector2(0, 9.81f);
        private Vector2 currentLiftSpeed;
        private Timer liftSpeedTimer;
        private const float liftSpeedGrace = 0.16f;

        public override Vector2 ExactPos
        {
            get => new Vector2(Pos.X + xRemainder, Pos.Y + yRemainder);
            set { Pos = VectorHelper.Floor(value);
                xRemainder = value.X - (float)Math.Floor(value.X);
                yRemainder = value.Y - (float)Math.Floor(value.Y);
            }
        }
        private float xRemainder;
        private float yRemainder;

        public Actor(Vector2 position, int width, int height, Sprite sprite)
            : base(position, width, height, sprite)
        {
            liftSpeedTimer = (Timer)AddComponent(new Timer(liftSpeedGrace, false, null, () => LiftSpeed = Vector2.Zero));
            liftSpeedTimer.Paused = true;
        }

        public virtual bool IsRiding(Solid solid)
            => Collider.CollideAt(solid, Pos + new Vector2(0, 1));

        public virtual void Squish()
            => Engine.CurrentMap.Destroy(this);

        public Vector2 LiftSpeed
        {
            get => currentLiftSpeed;

            set
            {
                currentLiftSpeed = value;

                if (value == Vector2.Zero)
                    return;
                liftSpeedTimer.Paused = false;
                liftSpeedTimer.Value = liftSpeedGrace;
            }
        }

        public void MoveX(float amount, Action CallbackOnCollision)
            => MoveX(amount, (entity) => CallbackOnCollision?.Invoke());

        public void MoveY(float amount, Action CallbackOnCollision)
            => MoveY(amount, (entity) => CallbackOnCollision?.Invoke());

        public void MoveX(float amount, Action<Entity> CallbackOnCollision = null)
            => MoveX(amount, new List<Entity>(Engine.CurrentMap.Data.Solids), CallbackOnCollision);

        public void MoveY(float amount, Action<Entity> CallbackOnCollision = null)
            => MoveY(amount, new List<Entity>(Engine.CurrentMap.Data.Solids), CallbackOnCollision);

        public void MoveX(float amount, List<Entity> checkedCollision, Action<Entity> CallbackOnCollision = null)
        {
            xRemainder += amount;
            int move = (int)Math.Floor(xRemainder);

            if (move != 0)
            {
                xRemainder -= move;
                int sign = Math.Sign(move);

                while (move != 0)
                {
                    if (!Collider.CollideAt(checkedCollision, Pos + new Vector2(sign, 0), out Entity collided))
                    {
                        Pos.X += sign;
                        move -= sign;
                    }
                    else
                    {
                        xRemainder = 0;
                        CallbackOnCollision?.Invoke(collided);
                        break;
                    }
                }
            }
        }

        public void MoveY(float amount, List<Entity> checkedCollision, Action<Entity> CallbackOnCollision = null)
        {
            yRemainder += amount;
            int move = (int)Math.Floor(yRemainder);

            if (move != 0)
            {
                yRemainder -= move;
                int sign = Math.Sign(move);

                while (move != 0)
                {
                    if (!Collider.CollideAt(checkedCollision, Pos + new Vector2(0, sign), out Entity collided))
                    {
                        Pos.Y += sign;
                        move -= sign;
                    }
                    else
                    {
                        yRemainder = 0;
                        CallbackOnCollision?.Invoke(collided);
                        break;
                    }
                }
            }
        }

        public void Move(Vector2 amount, Action CallbackOnCollisionX = null, Action CallbackOnCollisionY = null)
        {
            MoveX(amount.X, CallbackOnCollisionX);
            MoveY(amount.Y, CallbackOnCollisionY);
        }

        public void MoveTo(Vector2 pos, Action CallbackOnCollisionX = null, Action CallbackOnCollisionY = null) 
        {
            MoveX(pos.X - ExactPos.X, CallbackOnCollisionX);
            MoveY(pos.Y - ExactPos.Y, CallbackOnCollisionY);
        }
    }
}