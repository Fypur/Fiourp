using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public abstract class MovingSolid : Solid
    {
        public Vector2 Velocity;
        protected List<Actor> ridingActors;

        public override Vector2 ExactPos
        {
            get => new Vector2(Pos.X + xRemainder, Pos.Y + yRemainder); 
            set { Pos = VectorHelper.Floor(value); xRemainder = value.X - (float)Math.Floor(value.X); yRemainder = value.Y - (float)Math.Floor(value.Y); }
        }
        private float xRemainder;
        private float yRemainder;

        protected float gravityScale = 0;
        protected static readonly Vector2 gravityVector = new Vector2(0, 9.81f);

        public MovingSolid(Vector2 position, int width, int height, Sprite sprite) : base(position, width, height, sprite) { }
        public MovingSolid(Vector2 position, int width, int height, Color color) : base(position, width, height, color) { }

        public void Move(Vector2 vector)
            => Move(vector.X, vector.Y);

        public virtual void Move(float x, float y)
        {
            xRemainder += x;
            yRemainder += y;
            int moveX = (int)Math.Round(xRemainder);
            int moveY = (int)Math.Round(yRemainder);

            if (moveX == 0 && moveY == 0) return;

            List<Actor> ridingActors = GetAllRidingActors();

            Collider.Collidable = false;

            if (moveX != 0)
            {
                xRemainder -= moveX;
                Pos.X += moveX;
                
                if(moveX > 0)
                {
                    for (int i = Engine.CurrentMap.Data.Actors.Count - 1; i >= 0; i--)
                    {
                        Actor actor = Engine.CurrentMap.Data.Actors[i];
                        if (Collider.Collide(actor))
                        {
                            actor.MoveX(Pos.X + Width - actor.ExactPos.X, actor.Squish);
                            actor.LiftSpeed = new Vector2(moveX / Engine.Deltatime, actor.LiftSpeed.Y);
                            
                        }
                        else if (ridingActors.Contains(actor))
                        {
                            actor.MoveX(moveX);
                            actor.LiftSpeed = new Vector2(moveX / Engine.Deltatime, actor.LiftSpeed.Y);
                        }
                    }
                }
                else
                {
                    for (int i = Engine.CurrentMap.Data.Actors.Count - 1; i >= 0; i--)
                    {
                        Actor actor = Engine.CurrentMap.Data.Actors[i];
                        if (Collider.Collide(actor))
                        {
                            actor.MoveX(Pos.X - actor.ExactPos.X - actor.Width, actor.Squish);
                            actor.LiftSpeed = new Vector2(moveX / Engine.Deltatime, actor.LiftSpeed.Y);
                        }
                        else if (ridingActors.Contains(actor))
                        {
                            actor.MoveX(moveX);
                            actor.LiftSpeed = new Vector2(moveX / Engine.Deltatime, actor.LiftSpeed.Y);
                        }
                    }
                }
            }
            
            if(moveY != 0)
            {
                yRemainder -= moveY;
                Pos.Y += moveY;

                if (moveY > 0)
                {
                    for (int i = Engine.CurrentMap.Data.Actors.Count - 1; i >= 0; i--)
                    {
                        Actor actor = Engine.CurrentMap.Data.Actors[i];
                        if (Collider.Collide(actor))
                        {
                            actor.MoveY(Pos.Y + Height - actor.ExactPos.Y, actor.Squish);
                            actor.LiftSpeed = new Vector2(actor.LiftSpeed.X, moveY / Engine.Deltatime);
                        }
                        else if (ridingActors.Contains(actor))
                        {
                            actor.MoveY(moveY);
                            actor.LiftSpeed = new Vector2(actor.LiftSpeed.X, moveY / Engine.Deltatime);
                        }
                    }
                }
                else
                {
                    for (int i = Engine.CurrentMap.Data.Actors.Count - 1; i >= 0; i--)
                    {
                        Actor actor = Engine.CurrentMap.Data.Actors[i];
                        if (Collider.Collide(actor))
                        {
                            actor.MoveY(Pos.Y - actor.ExactPos.Y - actor.Height, actor.Squish);
                            actor.LiftSpeed = new Vector2(actor.LiftSpeed.X, moveY / Engine.Deltatime);
                        }
                        else if (ridingActors.Contains(actor))
                        {
                            actor.MoveY(moveY);
                            actor.LiftSpeed = new Vector2(actor.LiftSpeed.X, moveY / Engine.Deltatime);
                        }
                    }
                }
            }

            Collider.Collidable = true;
        }

        public void MoveCollideSolids(Vector2 amount, Action CallbackOnCollisionX = null, Action CallbackOnCollisionY = null)
            => MoveCollideSolids(amount.X, amount.Y, CallbackOnCollisionX, CallbackOnCollisionY);

        public void MoveCollideSolids(float amountX, float amountY, Action CallbackOnCollisionX = null, Action CallbackOnCollisionY = null)
        {
            float finalX = 0;
            float finalY = 0;

            xRemainder += amountX;
            int move = (int)Math.Round(xRemainder);

            if (move != 0)
            {
                xRemainder -= move;
                int sign = Math.Sign(amountX);

                while (move != 0)
                {
                    if (!Collider.CollideAt(Pos + new Vector2(finalX + sign, 0)))
                    {
                        finalX += sign;
                        move -= sign;
                    }
                    else
                    {
                        CallbackOnCollisionX?.Invoke();
                        break;
                    }
                }
            }

            yRemainder += amountY;
            move = (int)Math.Round(yRemainder);

            if (move != 0)
            {
                yRemainder -= move;
                int sign = Math.Sign(amountY);

                while (move != 0)
                {
                    if (!Collider.CollideAt(Pos + new Vector2(0, finalY + sign)))
                    {
                        finalY += sign;
                        move -= sign;
                    }
                    else
                    {
                        CallbackOnCollisionY?.Invoke();
                        break;
                    }
                }
            }

            Move(finalX, finalY);
        }

        public void MoveTo(Vector2 pos)
        {
            Move(pos.X - ExactPos.X, pos.Y - ExactPos.Y);
        }

        protected void Gravity()
            => Velocity += gravityVector * gravityScale;

        private List<Actor> GetAllRidingActors()
        {
            List<Actor> ridingActors = new List<Actor>();

            foreach(Actor a in Engine.CurrentMap.Data.Actors)
            {
                if (a.IsRiding(this))
                    ridingActors.Add(a);
            }

            return ridingActors;
        }
    }
}
