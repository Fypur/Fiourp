using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class CircleCollider : Collider
    {
        public float Radius { get => radiusPercentage * ParentEntity.Width; set => radiusPercentage = value / ParentEntity.Width; }
        private float radiusPercentage;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localPosition">Position in LOCAL space</param>
        /// <param name="radius"></param>
        public CircleCollider(Vector2 localPosition, float radius)
        {
            LocalPos = localPosition;
            radiusPercentage = radius;
        }

        public override void Added()
        {
            radiusPercentage = radiusPercentage / ParentEntity.Width;
        }

        public override void Render()
        {
            Drawing.DrawCircleEdge(WorldPos, Radius, 0.1f, DebugColor, 1);
        }

        public override bool Collide(AABBCollider other)
            => Collision.RectCircle(other.Bounds, WorldPos, Radius);

        public override bool Collide(BoxCollider other)
            => other.Collide(this);

        public override bool Collide(CircleCollider other)
            => Vector2.Distance(WorldPos, other.WorldPos) < Radius + other.Radius;

        public override bool Collide(Vector2 point)
            => Vector2.Distance(WorldPos, point) < Radius;

        public override bool Collide(GridCollider other)
        {
            throw new NotImplementedException();
        }

        public override float Width { get => Radius * 2; set => Radius = value * 0.5f; }
        public override float Height { get => Radius * 2; set => Radius = value * 0.5f; }
        public override float Left { get => LocalPos.X - Radius; set => LocalPos.X = value + Radius; }
        public override float Right { get => LocalPos.X + Radius; set => LocalPos.X = value - Radius; }
        public override float Top { get => LocalPos.Y - Radius; set => LocalPos.X = value + Radius; }
        public override float Bottom { get => LocalPos.Y + Radius; set => LocalPos.X = value - Radius; }
    }
}
