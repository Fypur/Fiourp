using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Fiourp
{
    public abstract class Collider : Component
    {
        public bool Collidable = true;

        public Vector2 Pos;
        public Color DebugColor = Color.Blue;
        public bool DebugDraw = true;

        public abstract bool Collide(Vector2 point);
        public abstract bool Collide(BoxCollider other);
        public abstract bool Collide(BoxColliderRotated other);
        public abstract bool Collide(CircleCollider other);
        public abstract bool Collide(GridCollider other);
        
        public abstract float Width { get; set; }
        public abstract float Height { get; set; }
        
        public abstract float Left { get; set; }
        public abstract float Right { get; set; }
        public abstract float Top { get; set; }
        public abstract float Bottom { get; set; }

        public virtual void Render() 
        {
            if(Collidable && DebugDraw)
                Drawing.DrawEdge(Bounds, 1, DebugColor);
        }

        public bool Collide(Collider other)
        {
            if (!other.Collidable || !ParentEntity.CollidingConditions(other) || !other.ParentEntity.CollidingConditions(this))
                return false;

            if (other is BoxCollider box)
                return Collide(box);
            if (other is BoxColliderRotated boxRot)
                return Collide(boxRot);
            if (other is CircleCollider circle)
                return Collide(circle);
            if(other is GridCollider grid)
                return Collide(grid);
            
            throw new Exception("Collider type has not been implemented yet.");
        }

        public bool Collide(Entity entity)
            => Collide(entity.Collider);

        public bool Collide(List<Collider> others)
        {
            foreach (Collider other in others)
                if (Collide(other))
                    return true;

            return false;
        }

        public bool CollideAt(List<Entity> checkedEntities, Vector2 position)
        {
            Vector2 oldPos = ParentEntity.Pos;
            ParentEntity.Pos = position;

            foreach (Entity e in checkedEntities)
                if (Collide(e) && e != ParentEntity)
                {
                    ParentEntity.Pos = oldPos;
                    return true;
                }

            ParentEntity.Pos = oldPos;
            return false;
        }

        public bool CollideAt(List<Entity> checkedEntities, Vector2 position, out Entity collidedEntity)
        {
            Vector2 oldPos = ParentEntity.Pos;
            ParentEntity.Pos = position;
            collidedEntity = null;

            foreach (Entity e in checkedEntities)
                if (Collide(e) && e != ParentEntity)
                {
                    ParentEntity.Pos = oldPos;
                    collidedEntity = e;
                    return true;
                }

            ParentEntity.Pos = oldPos;
            return false;
        }

        public bool CollideAt(Vector2 position)
            => CollideAt(new List<Entity>(Engine.CurrentMap.Data.Solids), position);

        public bool CollideWithActorsAt(Vector2 position)
            => CollideAt(new List<Entity>(Engine.CurrentMap.Data.Actors), position);

        public bool CollideAt(Vector2 position, out Entity entity)
        {
            bool returned = CollideAt(new List<Entity>(Engine.CurrentMap.Data.Entities), position, out entity);
            return returned;
        }

        public bool CollideAt(Vector2 position, out Actor actor)
        {
            bool returned = CollideAt(new List<Entity>(Engine.CurrentMap.Data.Actors), position, out Entity entity);
            actor = (Actor)entity;
            return returned;
        }

        public bool CollidedWithEntityOfType<T>(Vector2 pos, out T collidedEntity) where T : Entity
        {
            bool returned = CollideAt(new List<Entity>(Engine.CurrentMap.Data.EntitiesByType[typeof(T)]), pos, out Entity entity);
            collidedEntity = (T)entity;
            return returned;
        }

        public bool CollidedWithEntityOfType<T>(Vector2 pos)
            => CollideAt(new List<Entity>(Engine.CurrentMap.Data.EntitiesByType[typeof(T)]), pos);

        public bool CollideAt(Entity entity, Vector2 position)
            => CollideAt(new List<Entity>() { entity }, position);

        public float CenterX
        {
            get => Left + Width / 2;
            set => Left = value - Width / 2;
        }

        public float CenterY
        {
            get => Top + Height / 2;
            set => Top = value - Height / 2;
        }

        public Vector2 TopLeft
        {
            get => new Vector2(Left, Top);
            set { Left = value.X; Top = value.Y; }
        }

        public Vector2 TopRight
        {
            get => new Vector2(Right, Top);
            set { Right = value.X; Top = value.Y; }
        }

        public Vector2 BottomLeft
        {
            get => new Vector2(Left, Bottom);
            set { Left = value.X; Bottom = value.Y; }
        }

        public Vector2 BottomRight
        {
            get => new Vector2(Right, Bottom);
            set { Right = value.X; Bottom = value.Y; }
        }

        public Vector2 Center
        {
            get => new Vector2(Left + Width / 2, Top + Height / 2);
            set { Left = value.X - Width / 2; Top = value.Y - Height / 2; }
        }

        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set { Width = value.X; Height = value.Y; }
        }

        public Vector2 HalfSize
        {
            get => Size * 0.5f; 
        }

        public float AbsoluteX { get => ParentEntity.Pos.X + Pos.X; }
        public float AbsoluteY { get => ParentEntity.Pos.Y + Pos.Y; }
        public Vector2 AbsolutePosition { get => ParentEntity.Pos + Pos; }
        public float AbsoluteLeft { get => ParentEntity.Pos.X + Left; }
        public float AbsoluteRight { get => ParentEntity.Pos.X + Right; }
        public float AbsoluteTop { get => ParentEntity.Pos.Y + Top; }
        public float AbsoluteBottom { get => ParentEntity.Pos.Y + Bottom; }
        public Rectangle Bounds { get => new Rectangle((int)AbsoluteLeft, (int)AbsoluteTop, (int)Width, (int)Height); }
    }
}
