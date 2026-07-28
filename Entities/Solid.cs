using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Fiourp
{
    public abstract class Solid : Kinematic
    {
        protected AABBCollider AABBCollider => (AABBCollider)Collider;
        protected List<Actor> ridingActors;

        public Solid(Vector2 position, AABBCollider collider, Color color) : this(position, collider, new Sprite(color)) { }
        public Solid(Vector2 position, AABBCollider collider, Sprite sprite) : base(position, collider, sprite)
        { }

        public override Vector2 Move(Vector2 vector)
            => Move(vector.X, vector.Y, ParentMap.Actors);

        public void Move(float x, float y)
            => Move(x, y, ParentMap.Actors);

        public Vector2 Move(float x, float y, IEnumerable<Actor> actors)
        {
            xRemainder += x;
            yRemainder += y;

            foreach (Entity child in Children)
                child.Move(new Vector2(x, y));

            int moveX = (int)Math.Floor(xRemainder);
            int moveY = (int)Math.Floor(yRemainder);

            if (!Collider.Collidable)
            {
                Pos.X += moveX;
                xRemainder -= moveX;
                Pos.Y += moveY;
                yRemainder -= moveY;
                return new Vector2(x, y);
            }

            if (moveX == 0 && moveY == 0) return new Vector2(x, y);

            List<Actor> ridingActors = GetAllRidingActors(actors);
            List<Actor> ridingActorsX = new List<Actor>(ridingActors);

            Collider.Collidable = false;

            if (moveX != 0)
            {
                xRemainder -= moveX;
                Pos.X += moveX;

                foreach (Actor actor in actors)
                {
                    if (Collider.Collide(actor.Collider))
                    {
                        if (moveX > 0)
                            actor.MoveX(Pos.X + Collider.Bounds.Width - actor.Pos.X, (k) => actor.Squish());
                        else
                            actor.MoveX(Pos.X - actor.Pos.X - actor.Collider.Bounds.Width, (k) => actor.Squish());

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

                foreach (Actor actor in actors)
                {
                    if (Collider.Collide(actor.Collider))
                    {
                        if (moveY > 0)
                            actor.MoveY(Pos.Y + Collider.Bounds.Height - actor.Pos.Y, (k) => actor.Squish());
                        else
                            actor.MoveY(Pos.Y - actor.Pos.Y - actor.Collider.Bounds.Height, (k) => actor.Squish());

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
            return new Vector2(x, y);
        }

        public void MoveCollideSolids(Vector2 amount, Action CallbackOnCollisionX = null, Action CallbackOnCollisionY = null)
            => MoveCollideSolids(amount.X, amount.Y, ParentMap.Actors, ParentMap.NonActorKinematics, CallbackOnCollisionX, CallbackOnCollisionY);
        public void MoveCollideSolids(Vector2 amount, IEnumerable<Actor> actors, IEnumerable<Kinematic> otherKinematics, Action CallbackOnCollisionX = null, Action CallbackOnCollisionY = null)
            => MoveCollideSolids(amount.X, amount.Y, actors, otherKinematics, CallbackOnCollisionX, CallbackOnCollisionY);

        public void MoveCollideSolids(float amountX, float amountY, IEnumerable<Actor> actors, IEnumerable<Kinematic> otherSolids, Action CallbackOnCollisionX = null, Action CallbackOnCollisionY = null)
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
                    if (!CollideAt(otherSolids, Pos + new Vector2(finalX + sign, 0), out Kinematic other))
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
                    if (!CollideAt(otherSolids, Pos + new Vector2(0, finalY + sign), out Kinematic other))
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

            Move(finalX, finalY, actors);
        }

        private List<Actor> GetAllRidingActors(IEnumerable<Actor> actors)
        {
            List<Actor> ridingActors = new List<Actor>();

            foreach (Actor a in actors)
            {
                if (a.IsRiding(this))
                    ridingActors.Add(a);
            }

            return ridingActors;
        }

        public Solid AddChild(Solid child)
        {
            Children.Add(child);
            child.Parent = this;
            return child;
        }

        public void RemoveChild(Solid child)
        {
            Children.Remove(child);
            child.Parent = null;
        }
    }
}
