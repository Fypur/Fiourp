using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class AABBCollider : Collider
    {
        public int Width;
        public int Height;

        public override Rectangle Bounds => new Rectangle((int)WorldPos.X, (int)WorldPos.Y, Width, Height);

        public AABBCollider(Vector2 localPosition, int width, int height)
        {
            LocalPos = localPosition;
            Width = width;
            Height = height;
        }

        protected override bool CollideRaw(Collider other)
        {
            if(other is AABBCollider aabb)
                return Bounds.Intersects(other.Bounds);
            else if(other is BoxCollider box)
                return box.Collide(this);
            else if(other is CircleCollider circle)
                return Collision.RectCircle(Bounds, circle.WorldPos, circle.Radius);
            else if(other is GridCollider grid)
                return grid.Collide(this);
            else
                throw new NotImplementedException($"Collision from AABBCollider with {other.GetType().Name} is not yet implemented.");
        }

        public override bool Contains(Vector2 point)
            => Bounds.Contains(point);
    }
}
