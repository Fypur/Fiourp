using FMOD.Studio;
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
            public int AxisIndex;
        }

        public static SATOutput SAT(Vector2[] polygon1, Vector2[] polygon2, Vector2[] axies)
        {
            for (int i = 0; i < axies.Length; i++)
                axies[i].Normalize();

            SATOutput result = new SATOutput();
            result.IsCollision = true;
            result.Penetration = float.PositiveInfinity;

            for (int i = 0; i < axies.Length; i++)
            {
                float min1 = float.PositiveInfinity;
                float max1 = float.NegativeInfinity;
                float min2 = float.PositiveInfinity;
                float max2 = float.NegativeInfinity;

                foreach (Vector2 point in polygon1)
                {
                    float axisPos = Vector2.Dot(VectorHelper.Projection(point, axies[i]), axies[i]);
                    min1 = Math.Min(min1, axisPos);
                    max1 = Math.Max(max1, axisPos);
                }

                foreach (Vector2 point in polygon2)
                {
                    float axisPos = Vector2.Dot(VectorHelper.Projection(point, axies[i]), axies[i]);
                    min2 = Math.Min(min2, axisPos);
                    max2 = Math.Max(max2, axisPos);
                }

                if (min2 >= max1 || max2 <= min1)
                {
                    result.IsCollision = false;
                    result.MinPenetrationAxis = Vector2.Zero;
                    result.Penetration = 0;
                    result.AxisIndex = -1;
                    return result;
                }
                else
                {
                    if (max1 >= min2 && max1 - min2 <= max2 - min1 && max1 - min2 < result.Penetration)
                    {
                        result.Penetration = max1 - min2;
                        result.MinPenetrationAxis = axies[i];
                        result.AxisIndex = i;
                    }
                    else if(max2 - min1 < result.Penetration)
                    {
                        result.Penetration = max2 - min1;
                        result.MinPenetrationAxis = axies[i];
                        result.AxisIndex = i;
                    }
                }
            }

            result.IsCollision = true;
            return result;
        }

        public class BoxContact
        {
            public bool Colliding;

            public BoxColliderRotated Reference;
            public BoxColliderRotated Incident;
            public Vector2 Normal;

            public Vector2 ReferenceFace1;
            public Vector2 ReferenceFace2;
            public Vector2 ClippedIncidentFace1;
            public Vector2 ClippedIncidentFace2;
        }

        public static BoxContact BoxBoxClipping(BoxColliderRotated b1, BoxColliderRotated b2)
        {
            SATOutput sat = BoxBoxSAT(b1.Rect, b2.Rect);

            BoxContact contact = new BoxContact();
            contact.Colliding = true;
            bool xSatAxis;

            switch (sat.AxisIndex)
            {
                case 0:
                    contact.Reference = b1;
                    contact.Incident = b2;
                    xSatAxis = true;
                    break;
                case 1:
                    contact.Reference = b1;
                    contact.Incident = b2;
                    xSatAxis = false;
                    break;
                case 2:
                    contact.Reference = b2;
                    contact.Incident = b1;
                    xSatAxis = true;
                    break;
                case 3:
                    contact.Reference = b2;
                    contact.Incident = b1;
                    xSatAxis = false;
                    break;
                case -1:
                    contact.Colliding = false;
                    return contact;
                    throw new Exception("Clipping called even though there is no collision");
                default:
                    throw new Exception("sat axis index is not within expected bounds");
            }

            Vector2 refToInc = contact.Incident.Rect[0] + (contact.Incident.Rect[2] - contact.Incident.Rect[0]) * 0.5f - contact.Reference.Rect[0] - (contact.Reference.Rect[2] - contact.Reference.Rect[0]) * 0.5f;

            if (Vector2.Dot(refToInc, sat.MinPenetrationAxis) >= 0)
                contact.Normal = sat.MinPenetrationAxis;
            else
                contact.Normal = -sat.MinPenetrationAxis;

            if (xSatAxis)
            {
                if (Vector2.Dot(contact.Reference.Rect[1] - contact.Reference.Rect[0], contact.Normal) >= 0)
                {
                    contact.ReferenceFace1 = contact.Reference.Rect[1];
                    contact.ReferenceFace2 = contact.Reference.Rect[2];
                }
                else
                {
                    contact.ReferenceFace1 = contact.Reference.Rect[3];
                    contact.ReferenceFace2 = contact.Reference.Rect[0];
                }

            }
            else
            {
                if (Vector2.Dot(contact.Reference.Rect[0] - contact.Reference.Rect[3], contact.Normal) >= 0)
                {
                    contact.ReferenceFace1 = contact.Reference.Rect[0];
                    contact.ReferenceFace2 = contact.Reference.Rect[1];
                }
                else
                {
                    contact.ReferenceFace1 = contact.Reference.Rect[2];
                    contact.ReferenceFace2 = contact.Reference.Rect[3];
                }
            }

            Vector2 inc1, inc2; //incident face
            if (Math.Abs(Vector2.Dot(contact.Normal, (contact.Incident.Rect[1] - contact.Incident.Rect[0]).Normalized())) >= Math.Abs(Vector2.Dot(contact.Normal, (contact.Incident.Rect[2] - contact.Incident.Rect[1]).Normalized())))
            {
                //incident face is along y axis
                if (Vector2.Dot(contact.Normal, contact.Incident.Rect[1] - contact.Incident.Rect[0]) <= 0)
                {
                    inc1 = contact.Incident.Rect[1];
                    inc2 = contact.Incident.Rect[2];
                }
                else
                {
                    inc1 = contact.Incident.Rect[0];
                    inc2 = contact.Incident.Rect[3];
                }
            }
            else
            {
                //incident face is along x axis
                if (Vector2.Dot(contact.Normal, contact.Incident.Rect[1] - contact.Incident.Rect[2]) <= 0)
                {
                    inc1 = contact.Incident.Rect[0];
                    inc2 = contact.Incident.Rect[1];
                }
                else
                {
                    inc1 = contact.Incident.Rect[2];
                    inc2 = contact.Incident.Rect[3];
                }
            }

            contact.ClippedIncidentFace1 = VectorHelper.ClipBetween(contact.ReferenceFace1, contact.ReferenceFace2, inc1);
            contact.ClippedIncidentFace2 = VectorHelper.ClipBetween(contact.ReferenceFace1, contact.ReferenceFace2, inc2);

            Debug.PointUpdate(contact.ReferenceFace1, contact.ReferenceFace2, contact.ClippedIncidentFace1, contact.ClippedIncidentFace2);

            return contact;
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
                box2[1] - box2[0], //UL - UR
                box2[1] - box2[2], //UL - LL
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
