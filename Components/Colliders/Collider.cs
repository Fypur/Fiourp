using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Fiourp
{
    public abstract class Collider : Component
    {
        public bool Collidable = true;

        public Vector2 LocalPos;
        public Vector2 WorldPos { get => ParentEntity.Pos + LocalPos; }

        public Color DebugColor = Color.Blue;
        public bool DebugDraw = true;

        public abstract Rectangle Bounds { get; }

        public abstract bool CollideRaw(Collider other);
        public abstract bool Contains(Vector2 point);

        public bool Collide(Collider other)
        {
            if (!other.Collidable || !ParentEntity.CollidingConditions(other) || !other.ParentEntity.CollidingConditions(this))
                return false;

            return CollideRaw(other);
        }

        public bool Collide(Entity entity)
            => Collide(entity.Collider);

        public bool CollideAt(Vector2 position)
            => CollideAt(new List<Entity>(Engine.CurrentMap.Data.Solids), position, out _);

        public bool CollideAt(List<Entity> checkedEntities, Vector2 position)
            => CollideAt(checkedEntities, position, out _);

        public bool CollideAt(Entity entity, Vector2 position)
            => CollideAt(new List<Entity>() { entity }, position);

        public bool CollideAt(List<Entity> checkedEntities, Vector2 position, out Entity collidedEntity)
        {
            Vector2 oldPos = ParentEntity.Pos;
            ParentEntity.Pos = position;
            Update();
            collidedEntity = null;

            foreach (Entity e in checkedEntities)
                if (Collide(e) && e != ParentEntity)
                {
                    ParentEntity.Pos = oldPos;
                    collidedEntity = e;
                    return true;
                }

            ParentEntity.Pos = oldPos;
            Update();
            return false;
        }

        public override void Render()
        {
#if DEBUG

            if (Collidable && Debug.DebugMode)
                DebugRender();
#endif
        }

        protected virtual void DebugRender()
            => Drawing.DrawEdge(Bounds, 1, DebugColor);

    }
}
