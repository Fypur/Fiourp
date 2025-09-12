using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
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

            if (distX <= halfSize.X || distY <= halfSize.Y)
                return true;

            if (distX * distX + distY * distY < cRadius * cRadius)
                return true;

            return false;
        }
        
        public class SATOutput
        {
            public bool IsCollision;
            public Vector2 MinPenetrationAxis;
            public float Penetration;
        }

        public static SATOutput SAT(Vector2[] polygon1, Vector2[] polygon2, Vector2[] axies)
        {
            for (int i = 0; i < axies.Length; i++)
                axies[i].Normalize();
            SATOutput result = new SATOutput();
            result.IsCollision = true;
            result.Penetration = float.PositiveInfinity;

            foreach (Vector2 axis in axies)
            {
                float min1 = float.PositiveInfinity;
                float max1 = float.NegativeInfinity;
                float min2 = float.PositiveInfinity;
                float max2 = float.NegativeInfinity;

                foreach (Vector2 point in polygon1)
                {
                    float axisPos = Vector2.Dot(VectorHelper.Projection(point, axis), axis);
                    min1 = Math.Min(min1, axisPos);
                    max1 = Math.Max(max1, axisPos);
                }

                foreach (Vector2 point in polygon2)
                {
                    float axisPos = Vector2.Dot(VectorHelper.Projection(point, axis), axis);
                    min2 = Math.Min(min2, axisPos);
                    max2 = Math.Max(max2, axisPos);
                }

                if (min2 >= max1 || max2 <= min1)
                {
                    result.IsCollision = false;
                    result.MinPenetrationAxis = Vector2.Zero;
                    result.Penetration = 0;
                    return result;
                }
                else
                {
                    
                    if (max1 >= min2 && max1 - min2 <= max2 - min1 && max1 - min2 < result.Penetration)
                    {
                        result.Penetration = max1 - min2;
                        result.MinPenetrationAxis = axis;
                    }
                    else if(max2 - min1 < result.Penetration)
                    {
                        result.Penetration = max2 - min1;
                        result.MinPenetrationAxis = axis;
                    }
                }
            }

            result.IsCollision = true;
            return result;
        }

        public class ContactPoint
        {
            public SATOutput Sat;
            public Vector2 Point;
            public Vector2 Axis;
            public float NormalImpulse;
            public float FrictionImpulse;
        }

        public static ContactPoint BoxBoxClipping(BoxColliderRotated box1, BoxColliderRotated box2)
        {
            //SATOutput sat = BoxBoxSAT(box1.Rect, box2.Rect);

            //SAT -> Axis of least penetration + Min penetration
            //Identify reference and incident planes
            //Box-box clipping
            //Calc impulse
            //Bias impulse
            //Sequential (loop) and accumulated (memory) impulses
        }

        public static SATOutput BoxBoxSAT(Vector2[] box, Vector2[] box2)
        {
            if (box.Length != 4 || box2.Length != 4)
                throw new Exception("Rect vertices are not set properly, Rectangle has more or less than 4 vertices");
            
            //Indexes: UL = 0, UR = 1, LR = 2, LL = 3
            Vector2[] axies = new Vector2[4]
            {
                box[1] - box[0], //UR - UL
                box[1] - box[2], //UR - LR
                box2[0] - box2[3], //UL - LL
                box2[0] - box2[1] //UL - UR
            };

            return SAT(box, box2, axies);
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

        public static List<Vector2> LineBoxIntersection(BoxCollider b, Vector2 lineBegin, Vector2 lineEnd)
        {
            List<Vector2> intersection = new();

            Vector2? left = LineIntersection(b.AbsolutePosition, b.AbsolutePosition + new Vector2(0, b.Height), lineBegin, lineEnd);
            if (left != null) intersection.Add(left.Value);

            Vector2? top = LineIntersection(b.AbsolutePosition, b.AbsolutePosition + new Vector2(b.Width, 0), lineBegin, lineEnd);
            if (top != null) intersection.Add(top.Value);

            Vector2? right = LineIntersection(b.AbsolutePosition + new Vector2(b.Width, 0), b.AbsolutePosition + new Vector2(b.Width, b.Height), lineBegin, lineEnd);
            if (right != null) intersection.Add(right.Value);

            Vector2? bottom = LineIntersection(b.AbsolutePosition + new Vector2(0, b.Height), b.AbsolutePosition + new Vector2(b.Width, b.Height), lineBegin, lineEnd);
            if (bottom != null) intersection.Add(bottom.Value);

            return intersection;
        }

        public static bool LineBoxCollision(BoxCollider b, Vector2 lineBegin, Vector2 lineEnd)
        {
            if (b.Collide(lineBegin) && b.Collide(lineEnd))
                return true;

            return LineBoxIntersection(b, lineBegin, lineEnd).Count != 0;
        }

        public static Vector2[] LineCircleIntersection(Vector2 lineBegin, Vector2 lineEnd, Vector2 circleCenter, float circleRadius)
        {
            Vector2 d = lineEnd - lineBegin;
            Vector2 f = lineBegin - circleCenter;

            float a = Vector2.Dot(d, d);
            float b = 2 * Vector2.Dot(f, d);
            float c = Vector2.Dot(f, f) - circleRadius * circleRadius;

            float delta = b * b - 4 * a * c;

            if (delta < 0.0001f)
                return new Vector2[0];

            delta = (float)Math.Sqrt(delta);

            float t1 = (-b + delta) / (2 * a);
            if (delta == 0)
                return new Vector2[1] { lineBegin + d * t1 };
            
            float t2 = (-b - delta) / (2 * a);
            

            if(t1 >= 0 && t1 <= 1)
            {
                if(t2 >= 0 && t2 <= 1)
                    return new Vector2[2] { lineBegin + d * t1, lineBegin + d * t2 };
                return new Vector2[1] { lineBegin + d * t1 };
            }

            if(t2 >= 0 && t2 <= 1)
                return new Vector2[1] { lineBegin + d * t2 };

            return new Vector2[0];
        }
    }
}
