using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Fiourp
{
    public abstract class Solid : Kinematic
    {
        public static List<Solid> InstantiatedSolids = new();

        protected AABBCollider AABBCollider => (AABBCollider)Collider;
        protected List<Actor> ridingActors;

        public Solid(Vector2 position, AABBCollider collider, Color color) : this(position, collider, new Sprite(color)) { }
        public Solid(Vector2 position, AABBCollider collider, Sprite sprite) : base(position, collider, sprite)
        { }

        public override void Awake()
        {
            base.Awake();
            InstantiatedSolids.Add(this);
        }

        public override void OnDestroy()
        {
            InstantiatedSolids.Remove(this);
            base.OnDestroy();
        }

        public override void Move(Vector2 vector)
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

                for (int i = Actor.InstantiatedActors.Count - 1; i >= 0; i--)
                {
                    Actor actor = Actor.InstantiatedActors[i];
                    if (Collider.Collide(actor.Collider))
                    {
                        if (moveX > 0)
                            actor.MoveX(Pos.X + Collider.Bounds.Width - actor.Pos.X, actor.Squish);
                        else
                            actor.MoveX(Pos.X - actor.Pos.X - actor.Collider.Bounds.Width, actor.Squish);

                        actor.LiftSpeed = new Vector2(moveX / Engine.Deltatime, actor.LiftSpeed.Y);
                        if (ridingActorsX.Contains(actor))
                            ridingActorsX.Remove(actor);
                    }
                }

                foreach (Actor actor in ridingActorsX)
                {
                    actor.MoveX(moveX);
                    actor.LiftSpeed = new Vector2(moveX / Engine.Deltatime, actor.LiftSpeed.Y);
                }
            }

            if (moveY != 0)
            {
                yRemainder -= moveY;
                Pos.Y += moveY;

                for (int i = Actor.InstantiatedActors.Count - 1; i >= 0; i--)
                {
                    Actor actor = Actor.InstantiatedActors[i];
                    if (Collider.Collide(actor.Collider))
                    {
                        if (moveY > 0)
                            actor.MoveY(Pos.Y + Collider.Bounds.Height - actor.Pos.Y, actor.Squish);
                        else
                            actor.MoveY(Pos.Y - actor.Pos.Y - actor.Collider.Bounds.Height, actor.Squish);

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
                    if (!CollideAt(new List<Kinematic>(InstantiatedSolids), Pos + new Vector2(finalX + sign, 0), out Kinematic other))
                    {
                        finalX += sign;
                        move -= sign;
                    }
                    else
                    {
                        xRemainder = 0;
                        CallbackOnCollisionX?.Invoke();
                        break;
                    }
                }
            }

            yRemainder += amountY;
            move = (int)Math.Floor(yRemainder);

            if (move != 0)
            {
                yRemainder -= move;
                int sign = Math.Sign(amountY);

                while (move != 0)
                {
                    if (!CollideAt(new List<Kinematic>(InstantiatedSolids), Pos + new Vector2(0, finalY + sign), out Kinematic other))
                    {
                        finalY += sign;
                        move -= sign;
                    }
                    else
                    {
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

        private List<Actor> GetAllRidingActors()
        {
            List<Actor> ridingActors = new List<Actor>();

            foreach (Actor a in Actor.InstantiatedActors)
            {
                if (a.IsRiding(this))
                    ridingActors.Add(a);
            }

            return ridingActors;
        }
    }
}
