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
            Pos = localPosition;
            radiusPercentage = radius;
        }

        public override void Added()
        {
            radiusPercentage = radiusPercentage / ParentEntity.Width;
        }

        public override void Render()
        {
            Drawing.DrawCircleEdge(AbsolutePosition, Radius, 0.1f, Color.Blue, 1);
        }

        public override bool Collide(BoxCollider other)
            => Collision.RectCircle(other.Bounds, AbsolutePosition, Radius);

        public override bool Collide(CircleCollider other)
            => Vector2.Distance(AbsolutePosition, other.AbsolutePosition) < Radius + other.Radius;

        public override bool Collide(Vector2 point)
            => Vector2.Distance(AbsolutePosition, point) < Radius;

        public override float Width { get => Radius * 2; set => Radius = value * 0.5f; }
        public override float Height { get => Radius * 2; set => Radius = value * 0.5f; }
        public override float Left { get => Pos.X - Radius; set => Pos.X = value + Radius; }
        public override float Right { get => Pos.X + Radius; set => Pos.X = value - Radius; }
        public override float Top { get => Pos.Y - Radius; set => Pos.X = value + Radius; }
        public override float Bottom { get => Pos.Y + Radius; set => Pos.X = value - Radius; }
    }
}
