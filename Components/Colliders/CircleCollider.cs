using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class CircleCollider : Collider
    {
        public float Radius;

        public override Rectangle Bounds => new Rectangle((int)(WorldPos.X - Radius), (int)(WorldPos.Y - Radius), (int)(Radius * 2), (int)(Radius * 2));

        public CircleCollider(Vector2 localPosition, float radius)
        {
            LocalPos = localPosition;
            Radius = radius;
        }

        public override bool CollideRaw(Collider other)
        {
            if(other is AABBCollider aabb)
                return Collision.RectCircle(other.Bounds, WorldPos, Radius);
            else if(other is BoxCollider box)
                return other.CollideRaw(this);
            else if(other is CircleCollider circle)
                return Vector2.Distance(WorldPos, other.WorldPos) < Radius + circle.Radius;
            else
                throw new NotImplementedException();
        }

        public override void Render()
        {
            Drawing.DrawCircleEdge(WorldPos, Radius, 0.1f, DebugColor, 1);
        }

        public override bool Contains(Vector2 point)
            => Vector2.Distance(WorldPos, point) < Radius;
    }
}
