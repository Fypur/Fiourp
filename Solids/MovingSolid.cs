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

        public float GravityScale = 0;
        protected static readonly Vector2 gravityVector = new Vector2(0, 9.81f);

        public MovingSolid(Vector2 position, int width, int height, Sprite sprite) : base(position, width, height, sprite) { }
        public MovingSolid(Vector2 position, int width, int height, Color color) : base(position, width, height, color) { }

        public void Move(Vector2 vector)
            => Move(vector.X, vector.Y);

        public virtual void Move(float x, float y)
        {
            xRemainder += x;
            yRemainder += y;

            int moveX = (int)Math.Floor(xRemainder);
            int moveY = (int)Math.Floor(yRemainder);

            if (!Collider.Collidable)
            {
                Pos.X += moveX;
                xRemainder -= moveX;
                Pos.Y += moveY;
                yRemainder -= moveY;
                return;
            }

            if (moveX == 0 && moveY == 0) return;

            List<Actor> ridingActors = GetAllRidingActors();
            List<Actor> ridingActorsX = new List<Actor>(ridingActors);
            
            Collider.Collidable = false;

            if (moveX != 0)
            {
                xRemainder -= moveX;
                Pos.X += moveX;

                for (int i = Engine.CurrentMap.Data.Actors.Count - 1; i >= 0; i--)
                {
                    Actor actor = Engine.CurrentMap.Data.Actors[i];
                    if (Collider.Collide(actor))
                    {
                        if (moveX > 0)
                            actor.MoveX(Pos.X + Width - actor.Pos.X, actor.Squish);
                        else
                            actor.MoveX(Pos.X - actor.Pos.X - actor.Width, actor.Squish);

                        actor.LiftSpeed = new Vector2(moveX / Engine.Deltatime, actor.LiftSpeed.Y);
                        if(ridingActorsX.Contains(actor))
                            ridingActorsX.Remove(actor);
                    }
                }

                foreach (Actor actor in ridingActorsX)
                {
                    actor.MoveX(moveX);
                    actor.LiftSpeed = new Vector2(moveX / Engine.Deltatime, actor.LiftSpeed.Y);
                }
            }
            
            if(moveY != 0)
            {
                yRemainder -= moveY;
                Pos.Y += moveY;

                for (int i = Engine.CurrentMap.Data.Actors.Count - 1; i >= 0; i--)
                {
                    Actor actor = Engine.CurrentMap.Data.Actors[i];
                    if (Collider.Collide(actor))
                    {
                        if(moveY > 0)
                            actor.MoveY(Pos.Y + Height - actor.Pos.Y, actor.Squish);
                        else
                            actor.MoveY(Pos.Y - actor.Pos.Y - actor.Height, actor.Squish);

                        actor.LiftSpeed = new Vector2(actor.LiftSpeed.X, moveY / Engine.Deltatime);
                        if (ridingActors.Contains(actor))
                            ridingActors.Remove(actor);
                    }
                }

                foreach (Actor actor in ridingActors)
                {
                    actor.MoveY(moveY);
                    actor.LiftSpeed = new Vector2(actor.LiftSpeed.X, moveY / Engine.Deltatime);
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
            int move = (int)Math.Floor(xRemainder);

            if (move != 0)
            {
                xRemainder -= move;
                int sign = Math.Sign(amountX);

                while (move != 0)
                {
                    //if (!Collider.CollideAt(new List<Entity>(Engine.CurrentMap.Data.Solids), Pos + new Vector2(finalX + sign, 0), out Entity other))
                    if (!Collider.CollideAt(new List<Entity>(Engine.CurrentMap.Data.Platforms), Pos + new Vector2(finalX + sign, 0), out Entity other))
                    {
                        finalX += sign;
                        move -= sign;
                        
                    }
                    else
                    {
                        /*if (move > 0)
                            finalX = other.Pos.X - Pos.X - Width;
                        else
                            finalX = other.Pos.X + other.Width - Pos.X;*/

                        xRemainder = 0;
                        CallbackOnCollisionX?.Invoke();
                        break;
                    }
                }
            }

            //throw new Exception("The remainder is negative and it messes with positions with switches between positive and negative values");
            yRemainder += amountY;
            move = (int)Math.Floor(yRemainder);

            if (move != 0)
            {
                yRemainder -= move;
                int sign = Math.Sign(amountY);

                while (move != 0)
                {
                    //if (!Collider.CollideAt(new List<Entity>(Engine.CurrentMap.Data.Solids), Pos + new Vector2(0, finalY + sign), out Entity other))
                    if (!Collider.CollideAt(new List<Entity>(Engine.CurrentMap.Data.Platforms), Pos + new Vector2(0, finalY + sign), out Entity other))
                    {
                        finalY += sign;
                        move -= sign;
                    }
                    else
                    {
                        /*if (move > 0)
                            finalY = other.Pos.Y - Pos.Y - Height;
                        else
                            finalY = other.Pos.Y + other.Height - Pos.Y;*/

                        yRemainder = 0;
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
            => Velocity += gravityVector * GravityScale;

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
