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
        private Vector2 currentLiftSpeed;
        private Timer liftSpeedTimer;
        private const float liftSpeedGrace = 0.16f;

        public override Vector2 ExactPos
        {
            get => new Vector2(Pos.X + xRemainder, Pos.Y + yRemainder);
            set { Pos = VectorHelper.Floor(value); xRemainder = value.X - (float)Math.Floor(value.X); yRemainder = value.Y - (float)Math.Floor(value.Y); }
        }
        private float xRemainder;
        private float yRemainder;

        public Actor(Vector2 position, int width, int height, float gravityScale, Sprite sprite)
            : base(position, width, height, sprite)
        {
            this.gravityScale = gravityScale;
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

        public void MoveX(float amount, Action CallbackOnCollision = null)
        {
            xRemainder += amount;
            int move = (int)Math.Round(xRemainder);

            if (move != 0)
            {
                foreach (JumpThru jumpthru in Engine.CurrentMap.Data.GetEntities<JumpThru>())
                    jumpthru.Collider.Collidable = false;

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

                foreach (JumpThru jumpthru in Engine.CurrentMap.Data.GetEntities<JumpThru>())
                    jumpthru.Collider.Collidable = true;
            }
        }

        public void MoveY(float amount, Action CallbackOnCollision = null)
        {
            yRemainder += amount;
            int move = (int)Math.Round(yRemainder);

            if (move != 0)
            {
                yRemainder -= move;
                int sign = Math.Sign(amount);

                if (sign == -1)
                    foreach (JumpThru jumpthru in Engine.CurrentMap.Data.GetEntities<JumpThru>())
                        jumpthru.Collider.Collidable = false;

                while (move != 0)
                {
                    if (!Collider.CollideAt(Pos + new Vector2(0, sign)))
                    {
                        Pos.Y += sign;
                        move -= sign;
                    }
                    else
                    {
                        CallbackOnCollision?.Invoke();
                        break;
                    }
                }

                if (sign == -1)
                    foreach (JumpThru jumpthru in Engine.CurrentMap.Data.GetEntities<JumpThru>())
                        jumpthru.Collider.Collidable = true;
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

        public void Gravity()
        {
            Velocity += gravityVector * gravityScale;
        }
    }
}