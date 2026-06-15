using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Fiourp
{
    /// <summary>
    /// Entity that moves and collides with things
    /// </summary>
    public abstract class Actor : Kinematic
    {
        public static List<Actor> InstantiatedActors = new();

        private Vector2 currentLiftSpeed;
        private Timer liftSpeedTimer;
        private const float liftSpeedGrace = 0.16f;

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

        public Actor(Vector2 position, AABBCollider collider, Sprite sprite)
            : base(position, collider, sprite)
        {
            liftSpeedTimer = (Timer)AddComponent(new Timer(liftSpeedGrace, null, () => LiftSpeed = Vector2.Zero, false));
            liftSpeedTimer.Paused = true;
        }

        public virtual bool IsRiding(Solid solid)
            => CollideAt(solid, Pos + new Vector2(0, 1));

        public virtual void Squish()
            => Engine.CurrentMap.Destroy(this);

        public override void Awake()
        {
            base.Awake();
            InstantiatedActors.Add(this);
        }

        public override void OnDestroy()
        {
            InstantiatedActors.Remove(this);
            base.OnDestroy();
        }

        public void MoveX(float amount, Action callbackOnCollision)
            => MoveX(amount, (entity) => callbackOnCollision?.Invoke());

        public void MoveY(float amount, Action callbackOnCollision)
            => MoveY(amount, (entity) => callbackOnCollision?.Invoke());

        public void MoveX(float amount, Action<Kinematic> callbackOnCollision = null)
            => MoveX(amount, new List<Kinematic>(Solid.InstantiatedSolids), callbackOnCollision);

        public void MoveY(float amount, Action<Kinematic> callbackOnCollision = null)
            => MoveY(amount, new List<Kinematic>(Solid.InstantiatedSolids), callbackOnCollision);

        public void MoveX(float amount, List<Kinematic> checkedCollision, Action<Kinematic> callbackOnCollision = null)
        {
            xRemainder += amount;
            int move = (int)Math.Floor(xRemainder);

            if (move != 0)
            {
                xRemainder -= move;
                int sign = Math.Sign(move);

                while (move != 0)
                {
                    if (!CollideAt(checkedCollision, Pos + new Vector2(sign, 0), out Kinematic collided))
                    {
                        Pos.X += sign;
                        move -= sign;
                    }
                    else
                    {
                        xRemainder = 0;
                        callbackOnCollision?.Invoke(collided);
                        break;
                    }
                }
            }
        }

        public void MoveY(float amount, List<Kinematic> checkedCollision, Action<Kinematic> CallbackOnCollision = null)
        {
            yRemainder += amount;
            int move = (int)Math.Floor(yRemainder);

            if (move != 0)
            {
                yRemainder -= move;
                int sign = Math.Sign(move);

                while (move != 0)
                {
                    if (!CollideAt(checkedCollision, Pos + new Vector2(0, sign), out Kinematic collided))
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

        public override void Move(Vector2 moveAmount)
            => Move(moveAmount, null, null);

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