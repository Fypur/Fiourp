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

        private float xRemainder;
        private float yRemainder;

        protected float gravityScale = 0;
        protected static readonly Vector2 gravityVector = new Vector2(0, 9.81f);

        public MovingSolid(Vector2 position, int width, int height, Sprite sprite) : base(position, width, height, sprite) { }
        public MovingSolid(Vector2 position, int width, int height, Color color) : base(position, width, height, color) { }

        public void Move(float x, float y)
        {
            xRemainder += x;
            yRemainder += y;
            int moveX = (int)Math.Round(xRemainder);
            int moveY = (int)Math.Round(yRemainder);

            if (moveX == 0 && moveY == 0) return;

            List<Actor> ridingActors = GetAllRidingActors();
            Collider.Collidable = false;

            if(moveX != 0)
            {
                xRemainder -= moveX;
                Pos.X += moveX;

                if(moveX > 0)
                {
                    foreach (Actor actor in Engine.CurrentMap.Data.Actors)
                    {
                        if (Collider.Collide(actor))
                        {
                            actor.MoveX(Pos.X + Width - actor.Pos.X, actor.Squish);
                        }
                        else if (ridingActors.Contains(actor))
                        {
                            actor.MoveX(moveX, null);
                        }
                    }
                }
                else
                {
                    foreach (Actor actor in Engine.CurrentMap.Data.Actors)
                    {
                        if (Collider.Collide(actor))
                        {
                            actor.MoveX(Pos.X - actor.Pos.X - actor.Width, actor.Squish);
                        }
                        else if (ridingActors.Contains(actor))
                        {
                            actor.MoveX(moveX, null);
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
                    foreach (Actor actor in Engine.CurrentMap.Data.Actors)
                    {
                        if (Collider.Collide(actor))
                        {
                            actor.MoveY(Pos.Y + Height - actor.Pos.Y, actor.Squish);
                        }
                        else if (ridingActors.Contains(actor))
                        {
                            actor.MoveY(moveY, null);
                        }
                    }
                }
                else
                {
                    foreach (Actor actor in Engine.CurrentMap.Data.Actors)
                    {
                        if (Collider.Collide(actor))
                        {
                            actor.MoveY(Pos.Y - actor.Pos.Y - actor.Height, actor.Squish);
                        }
                        else if (ridingActors.Contains(actor))
                        {
                            actor.MoveY(moveY, null);
                        }
                    }
                }
            }

            Collider.Collidable = true;
        }

        protected void MoveTo(Vector2 pos)
        {
            Move(pos.X - Pos.X, pos.Y - Pos.Y);
        }

        void Gravity()
            => Velocity += gravityVector * gravityScale;

        private List<Actor> GetAllRidingActors()
        {
            List<Actor> ridingActors = new List<Actor>();

            foreach(Actor a in Engine.CurrentMap.Data.Actors)
            {
                if (a.IsRiding(this) == true)
                    ridingActors.Add(a);
            }

            return ridingActors;
        }
    }
}
