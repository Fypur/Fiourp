using Microsoft.Xna.Framework;

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
