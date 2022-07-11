using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class BoxCollider : Collider
    {
        private float widthPercentage;
        private float heightPercentage;

        /// <summary>
        ///
        /// </summary>
        /// <param name="localPosition">Position in LOCAL space</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public BoxCollider(Vector2 localPosition, int width, int height)
        {
            Pos = localPosition;
            widthPercentage = width;
            heightPercentage = height;
        }

        public override void Added()
        {
            widthPercentage = widthPercentage / ParentEntity.Width;
            heightPercentage = heightPercentage / ParentEntity.Height;
        }

        public override bool Collide(BoxCollider other)
            => Bounds.Intersects(other.Bounds);

        public override bool Collide(CircleCollider other)
            => Collision.RectCircle(Bounds, other.AbsolutePosition, other.Radius);

        public override bool Collide(Vector2 point)
            => Bounds.Contains(point);

        public override bool Collide(GridCollider other)
            => other.Collide(this);

        public override float Width { get => widthPercentage * ParentEntity.Width; set => widthPercentage = value / ParentEntity.Width; }
        public override float Height { get => heightPercentage * ParentEntity.Height; set => heightPercentage = value / ParentEntity.Height; }
        public override float Left { get => Pos.X; set => Pos.X = value; }
        public override float Right { get => Pos.X + Width; set => Pos.X = value - Width; }
        public override float Top { get => Pos.Y; set => Pos.Y = value; }
        public override float Bottom { get => Pos.Y + Height; set => Pos.Y = value - Height; }
    }
}
