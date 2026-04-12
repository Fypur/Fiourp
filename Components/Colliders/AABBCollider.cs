using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class AABBCollider : Collider
    {
        private float widthPercentage;
        private float heightPercentage;

        /// <summary>
        ///
        /// </summary>
        /// <param name="localPosition">Position in LOCAL space</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public AABBCollider(Vector2 localPosition, int width, int height)
        {
            LocalPos = localPosition;
            widthPercentage = width;
            heightPercentage = height;
        }

        public override void Added()
        {
            widthPercentage = widthPercentage / ParentEntity.Width;
            heightPercentage = heightPercentage / ParentEntity.Height;
        }

        public override bool Collide(AABBCollider other)
            => Bounds.Intersects(other.Bounds);

        public override bool Collide(CircleCollider other)
            => Collision.RectCircle(Bounds, other.WorldPos, other.Radius);
        

        public override bool Collide(Vector2 point)
            => Bounds.Contains(point);
        
        public override bool Collide(GridCollider other)
            => other.Collide(this);
        
        public override bool Collide(BoxCollider other)
            => other.Collide(this);

        public override float Width { get => widthPercentage * ParentEntity.Width; set => widthPercentage = value / ParentEntity.Width; }
        public override float Height { get => heightPercentage * ParentEntity.Height; set => heightPercentage = value / ParentEntity.Height; }
        public override float Left { get => LocalPos.X; set => LocalPos.X = value; }
        public override float Right { get => LocalPos.X + Width; set => LocalPos.X = value - Width; }
        public override float Top { get => LocalPos.Y; set => LocalPos.Y = value; }
        public override float Bottom { get => LocalPos.Y + Height; set => LocalPos.Y = value - Height; }
    }
}
