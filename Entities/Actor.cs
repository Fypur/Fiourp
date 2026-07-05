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

        public Actor(Vector2 position, Collider collider, Sprite sprite)
            : base(position, collider, sprite)
        {
            liftSpeedTimer = (Timer)AddComponent(new Timer(liftSpeedGrace, null, () => LiftSpeed = Vector2.Zero, false));
            liftSpeedTimer.Paused = true;
        }

        public virtual bool IsRiding(Solid solid)
            => CollideAt(solid, Pos + new Vector2(0, 1));

        public virtual void Squish()
            => SelfDestroy();

        public override void Move(Vector2 moveAmount)
        {
            Move(moveAmount.X, ParentMap.NonActorKinematics, true, null);
            Move(moveAmount.Y, ParentMap.NonActorKinematics, false, null);
        }

        public void MoveX(float amount)
            => MoveX(amount, ParentMap.NonActorKinematics, null);
        public void MoveY(float amount)
            => MoveY(amount, ParentMap.NonActorKinematics, null);
        public void MoveX(float amount, Action<Kinematic> callbackOnCollision)
            => MoveX(amount, ParentMap.NonActorKinematics, callbackOnCollision);
        public void MoveY(float amount, Action<Kinematic> callbackOnCollision)
            => MoveY(amount, ParentMap.NonActorKinematics, callbackOnCollision);
        public void MoveX(float amount, List<Kinematic> checkedCollision, Action<Kinematic> callbackOnCollision = null)
            => Move(amount, checkedCollision, true, callbackOnCollision);

        public void MoveY(float amount, List<Kinematic> checkedCollision, Action<Kinematic> callbackOnCollision = null)
            => Move(amount, checkedCollision, false, callbackOnCollision);

        private void Move(float amount, List<Kinematic> checkedCollision, bool xAxis, Action<Kinematic> callbackOnCollision = null)
        {
            ref float remainder = ref (xAxis ? ref xRemainder : ref yRemainder);
            float oldRemainder = remainder;

            remainder += amount;
            int move = (int)Math.Floor(remainder);

            if (move != 0)
            {
                remainder -= move;
                int sign = Math.Sign(move);
                Vector2 moveVector = xAxis ? new Vector2(sign, 0) : new Vector2(0, sign);

                while (move != 0)
                {
                    if (!CollideAt(checkedCollision, Pos + moveVector, out Kinematic collided))
                    {
                        if (xAxis)
                            Pos.X += sign;
                        else
                            Pos.Y += sign;

                        move -= sign;
                    }
                    else
                    {
                        remainder = 0;
                        callbackOnCollision?.Invoke(collided);
                        break;
                    }
                }
            }

            /*Vector2 childMove = xAxis ? new Vector2(remainder - oldRemainder, 0) : new Vector2(0, remainder - oldRemainder);
            foreach (Entity child in Children)
                child.Move(childMove);*/
        }
    }
}