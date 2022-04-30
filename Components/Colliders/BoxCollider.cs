using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class BoxCollider : Collider
    {
        private float width;
        private float height;

        /// <summary>
        ///
        /// </summary>
        /// <param name="localPosition">Position in LOCAL space</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public BoxCollider(Vector2 localPosition, int width, int height)
        {
            Pos = localPosition;
            this.width = width;
            this.height = height;
        }
        
        public override bool Collide(BoxCollider other)
            => Bounds.Intersects(other.Bounds);

        public override bool Collide(CircleCollider other)
            => Collision.RectCircle(Bounds, other.AbsolutePosition, other.Radius);

        public override bool Collide(Vector2 point)
            => Bounds.Contains(point);

        public override float Width { get => width; set => width = value; }
        public override float Height { get => height; set => height = value; }
        public override float Left { get => Pos.X; set => Pos.X = value; }
        public override float Right { get => Pos.X + width; set => Pos.X = value - width; }
        public override float Top { get => Pos.Y; set => Pos.Y = value; }
        public override float Bottom { get => Pos.Y + height; set => Pos.Y = value - height; }

    }
}
