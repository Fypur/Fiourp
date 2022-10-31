using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public static class Collision
    {
        public static bool RectCircle(Rectangle rect, Vector2 cPosition, float cRadius)
        {
            Vector2 halfSize = new Vector2(rect.Width / 2, rect.Height / 2);

            float distX = Math.Abs(cPosition.X - (rect.Left + halfSize.X));
            float distY = Math.Abs(cPosition.Y - (rect.Top + halfSize.Y));

            if (distX >= cRadius + halfSize.X || distY >= cRadius + halfSize.Y)
                return false;
            if (distX < halfSize.X && distY < halfSize.Y)
                return true;

            distX -= halfSize.X;
            distY -= halfSize.Y;

            if(distX * distX + distY * distY < cRadius * cRadius)
                return true;

            return false;
        }

        public static bool SeparatingAxisTheorem(Vector2[] box, Vector2[] other)
        {
            if (box.Length != 4 || other.Length != 4)
                throw new Exception("Rect vertices are not set properly, Rectangle has more or less than 4 vertices");
            
            //Indexes: UL = 0, UR = 1, LR = 2, LL = 3

            Vector2[] Axies = new Vector2[4]
            {
                box[1] - box[0], //UR - UL
                box[1] - box[2], //UR - LR
                other[0] - other[3], //UL - LL
                other[0] - other[1] //UL - UR
            };

            foreach (Vector2 axis in Axies)
            {
                float min1 = float.PositiveInfinity;
                float max1 = float.NegativeInfinity;
                float min2 = float.PositiveInfinity;
                float max2 = float.NegativeInfinity;

                foreach (Vector2 point in box)
                {
                    float axisPos = Vector2.Dot(VectorHelper.Projection(point, axis), axis);
                    min1 = Math.Min(min1, axisPos);
                    max1 = Math.Max(max1, axisPos);
                }

                foreach (Vector2 point in other)
                {
                    float axisPos = Vector2.Dot(VectorHelper.Projection(point, axis), axis);
                    min2 = Math.Min(min2, axisPos);
                    max2 = Math.Max(max2, axisPos);
                }

                if (!(min2 <= max1 && max2 >= min1))
                    return false;
            }

            return true;
        }

        public static Vector2? LineIntersection(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Vector2 s1 = p1 - p0;
            Vector2 s2 = p3 - p2;

            float s = (-s1.Y * (p0.X - p2.X) + s1.X * (p0.Y - p2.Y)) / (-s2.X * s1.Y + s1.X * s2.Y);
            float t = ( s2.X * (p0.Y - p2.Y) - s2.Y * (p0.X - p2.X)) / (-s2.X * s1.Y + s1.X * s2.Y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Collision detected
                return p0 + (t * s1);
            }

            return null; // No collision
        }

        public static Vector2[] LineCircleIntersection(Vector2 lineBegin, Vector2 lineEnd, Vector2 circleCenter, float circleRadius)
        {
            Vector2 d = (lineEnd - lineBegin);
            Vector2 f = (lineBegin - circleCenter);

            float a = Vector2.Dot(d, d);
            float b = 2 * Vector2.Dot(f, d);
            float c = Vector2.Dot(f, f) - circleRadius * circleRadius;

            float delta = b * b - 4 * a * c;

            if (delta < 0)
                return new Vector2[0];

            delta = (float)Math.Sqrt(delta);

            float t1 = (-b + delta) / (2 * a);
            if (delta == 0)
                return new Vector2[1] { lineBegin + d * t1 };
            
            float t2 = (-b - delta) / (2 * a);
            
            if(t2 < 0 || t2 > 1)
                return new Vector2[1] { lineBegin + d * t1 };
            if(t1 < 0 || t1 > 1)
                return new Vector2[1] { lineBegin + d * t2 };

            return new Vector2[2] { lineBegin + d * t1, lineBegin + d * t2 };
        }
    }
}
