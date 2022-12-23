using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Text;

namespace Fiourp
{
    public static class Polygon
    {
        public static bool IsPointInPolygon(Vector2[] polygon, Vector2 point)
        {
            float MinX = float.PositiveInfinity, MinY = float.PositiveInfinity, MaxX = float.NegativeInfinity, MaxY = float.NegativeInfinity;
            foreach(Vector2 p in polygon)
            {
                if(p.X < MinX)
                    MinX = p.X;
                if(p.X > MaxX)
                    MaxX = p.X;
                if(p.Y < MinY)
                    MinY = p.Y;
                if(p.Y > MaxY)
                    MaxY = p.Y;
            }

            if (point.X < MinX || point.X > MaxX || point.Y < MinY || point.Y > MaxY)
                return false;

            int I = 0;
            int J = polygon.Length - 1;
            bool IsMatch = false;

            for (; I < polygon.Length; J = I++)
            {
                //When the position is right on a point, count it as a match.
                if (polygon[I].X == point.X && polygon[I].Y == point.Y)
                    return true;
                if (polygon[J].X == point.X && polygon[J].Y == point.Y)
                    return true;

                //When the position is on a horizontal or vertical line, count it as a match.
                if (polygon[I].X == polygon[J].X && point.X == polygon[I].X && point.Y >= Math.Min(polygon[I].Y, polygon[J].Y) && point.Y <= Math.Max(polygon[I].Y, polygon[J].Y))
                    return true;
                if (polygon[I].Y == polygon[J].Y && point.Y == polygon[I].Y && point.X >= Math.Min(polygon[I].X, polygon[J].X) && point.X <= Math.Max(polygon[I].X, polygon[J].X))
                    return true;

                if (((polygon[I].Y > point.Y) != (polygon[J].Y > point.Y)) && (point.X < (polygon[J].X - polygon[I].X) * (point.Y - polygon[I].Y) / (polygon[J].Y - polygon[I].Y) + polygon[I].X))
                    IsMatch = !IsMatch;
            }

            return IsMatch;
        }

        public static PolygonPoint[] GetCircleVisibilityPolygon(Vector2 middle, float distance)
        {
            //Obtenir tous les corners à l'interieur du cercle
            //Cast sur les corners pour éliminer
            /*Vector2 middle = MiddlePos;
            float sqrdMaxedDist = MaxSwingDistance * MaxSwingDistance;*/
            float sqrdMaxedDist = distance * distance;



            List<Vector2> allCorners = new List<Vector2>(Engine.CurrentMap.CurrentLevel.Corners);
            allCorners.AddRange(Engine.CurrentMap.CurrentLevel.InsideCorners);
            List<Vector2> corners = new();
            Dictionary<Vector2, float> distancesSquared = new();


            foreach (Vector2 corner in allCorners)
            {
                float d = Vector2.DistanceSquared(middle, corner);

                if (d > sqrdMaxedDist)
                    continue;

                //Les 5 raycast comme ça c'est juste pcq des fois c'est un peu funky et un seul raycast trouve que y a collision
                //au pixel près. Donc j'en fait 5 pcq un raycast sur maptiles c'est pas expensive
                Raycast r = new Raycast(Raycast.RayTypes.MapTiles, middle, corner, true);
                if (r.Hit)
                {
                    r.Hit = Vector2.DistanceSquared(r.EndPoint, corner) > 1.1f;
                    //Debug.LogUpdate(r.Hit);
                }

                //Raycast bestRay = Raycast.FiveRays(middle, corner, false, false, 0.001f, true);

                if (!r.Hit)
                {
                    corners.Add(corner);
                    distancesSquared[corner] = d;
                }
            }

            //Cast derrière les corners pour voir les points aux edges du cercle
            List<Vector2> points = new();
            foreach (Vector2 corner in corners)
            {
                points.Add(corner);

                var r = new Raycast(Raycast.RayTypes.MapTiles, middle, corner - middle, distance, true);
                var r2 = new Raycast(Raycast.RayTypes.MapTiles, middle, corner - middle - Vector2.UnitX * 0.1f - Vector2.UnitY * 0.1f, distance, true);
                var r3 = new Raycast(Raycast.RayTypes.MapTiles, middle, corner - middle - Vector2.UnitX * 0.1f + Vector2.UnitY * 0.1f, distance, true);
                var r4 = new Raycast(Raycast.RayTypes.MapTiles, middle, corner - middle + Vector2.UnitX * 0.1f - Vector2.UnitY * 0.1f, distance, true);
                var r5 = new Raycast(Raycast.RayTypes.MapTiles, middle, corner - middle + Vector2.UnitX * 0.1f + Vector2.UnitY * 0.1f, distance, true);
                //Debug.PointUpdate(Color.DarkGreen, r5.EndPoint);

                Raycast bestRay = r;
                if (r2.DistanceSquared >= bestRay.DistanceSquared)
                    bestRay = r2;
                if (r3.DistanceSquared >= bestRay.DistanceSquared)
                    bestRay = r3;
                if (r4.DistanceSquared >= bestRay.DistanceSquared)
                    bestRay = r4;
                if (r5.DistanceSquared >= bestRay.DistanceSquared)
                    bestRay = r5;

                if (bestRay.DistanceSquared > distancesSquared[corner])
                {
                    distancesSquared[bestRay.EndPoint] = bestRay.DistanceSquared;
                    points.Add(bestRay.EndPoint);
                }
            }

            //On determine toutes les edges (non opti là ça fait par tile)
            List<int[]> edges = Engine.CurrentMap.CurrentLevel.Edges;

            //On cherche les points d'intersection sur les edges, puis on vérifie par raycast
            foreach (int[] edge in edges)
            {
                Vector2 coord1 = new Vector2(edge[0], edge[1]) * 8 + Engine.CurrentMap.CurrentLevel.Pos;
                Vector2 coord2 = new Vector2(edge[2], edge[3]) * 8 + Engine.CurrentMap.CurrentLevel.Pos;

                if (Vector2.DistanceSquared(middle, coord1) > sqrdMaxedDist && Vector2.DistanceSquared(middle, coord2) > sqrdMaxedDist)
                    continue;

                //Debug.PointUpdate(Color.Blue, coord1, coord2);
                Vector2[] collPoints = Collision.LineCircleIntersection(coord1, coord2, middle, distance);
                foreach (Vector2 p in collPoints)
                {
                    if (points.Contains(p))
                        continue;

                    //Raycast bestRay = Raycast.FiveRays(middle, p, false, true, 0.001f);

                    Raycast r = new Raycast(Raycast.RayTypes.MapTiles, middle, p, true);
                    if (r.Hit)
                    {
                        r.Hit = Vector2.DistanceSquared(r.EndPoint, p) > 1.1f;
                        //Debug.LogUpdate(r.Hit);
                    }
                    //Debug.PointUpdate(Color.Green, collPoints);

                    if (!r.Hit)
                    {
                        distancesSquared[p] = Vector2.DistanceSquared(middle, p);
                        points.Add(p);
                    }
                }
            }

            //On sort les points par angle
            Dictionary<Vector2, float> angles = new();
            foreach (Vector2 point in points)
                angles[point] = (point - middle).ToAngle();

            points.Sort((vec1, vec2) =>
            {
                return Math.Sign(angles[vec1] - angles[vec2]);
            });

            float CeilingOrFloor(float f)
            {
                if (f >= 0)
                    return (float)Math.Floor(f);
                return (float)Math.Ceiling(f);
            }

            Vector2 CeilingOrFloorV(Vector2 v)
                => new Vector2(CeilingOrFloor(v.X), CeilingOrFloor(v.Y));

            void CompareAndRemove(int index, int index2)
            {
                //if (Vector2.DistanceSquared(points[index], points[index2]) < 1)
                if(CeilingOrFloor(points[index].X) == CeilingOrFloor(points[index2].X) && CeilingOrFloor(points[index].Y) == CeilingOrFloor(points[index2].Y))
                {
                    if (distancesSquared[points[index2]] < distancesSquared[points[index]])
                        points.RemoveAt(index2);
                    else
                        points.RemoveAt(index);
                }
                return;

                /*if ()
                    points.RemoveAt(index2);*/
            }
              
            for (int i = 0; i < points.Count - 2; i++)
            {
                CompareAndRemove(i, i + 1);
                if (i < points.Count - 2)
                    CompareAndRemove(i, i + 2);
            }

            if (points.Count > 1)
            {
                CompareAndRemove(0, points.Count - 1);
                CompareAndRemove(1, points.Count - 1);
            }
            if (points.Count > 2)
            {
                CompareAndRemove(points.Count - 2, points.Count - 1);
                CompareAndRemove(0, points.Count - 2);
            }

            int CheckAngleAndChange(int index0, int index1, int index2)
            {
                bool Aligned(int index, int index2)
                    => CeilingOrFloorV(points[index] - points[index2]).X == 0 || CeilingOrFloorV(points[index] - points[index2]).Y == 0;

                bool SameEdge(int index, int index2)
                {
                    bool AlignedWithEdge(Vector2 edgeBegin, Vector2 edgeEnd)
                    {
                        return (CeilingOrFloorV(points[index] - edgeBegin).X == 0 || CeilingOrFloorV(points[index] - edgeBegin).Y == 0)
                            && (CeilingOrFloorV(points[index] - edgeEnd).X == 0 || CeilingOrFloorV(points[index] - edgeEnd).Y == 0)
                            && (CeilingOrFloorV(points[index2] - edgeBegin).X == 0 || CeilingOrFloorV(points[index2] - edgeBegin).Y == 0)
                            && (CeilingOrFloorV(points[index2] - edgeEnd).X == 0 || CeilingOrFloorV(points[index2] - edgeEnd).Y == 0);
                    }

                    if (!Aligned(index, index2))
                        return false;

                    foreach (int[] edge in edges)
                    {
                        Vector2 p1 = points[index];
                        Vector2 p2 = points[index2];
                        Vector2 edgeBegin = Engine.CurrentMap.CurrentLevel.GetOrganisationPos(edge[0], edge[1]);
                        Vector2 edgeEnd = Engine.CurrentMap.CurrentLevel.GetOrganisationPos(edge[2], edge[3]);

                        if (!AlignedWithEdge(edgeBegin, edgeEnd))
                            continue;

                        if (CeilingOrFloor(p1.X) >= edgeBegin.X && CeilingOrFloor(p1.Y) >= edgeBegin.Y && CeilingOrFloor(p2.X) >= edgeBegin.X && CeilingOrFloor(p2.Y) >= edgeBegin.Y &&
                            Vector2.DistanceSquared(edgeBegin, CeilingOrFloorV(p1)) <= Vector2.DistanceSquared(edgeBegin, edgeEnd) &&
                            Vector2.DistanceSquared(edgeBegin, CeilingOrFloorV(p2)) <= Vector2.DistanceSquared(edgeBegin, edgeEnd))
                            return true;
                    }

                    return false;
                }

                bool sameAngle = Math.Round(angles[points[index1]], 2) == Math.Round(angles[points[index2]], 2);

                if (!sameAngle)
                    return 1;

                //Points 0 and 2 are aligned and Points 0 and 2 are on circle edges
                if (!SameEdge(index0, index2) && !(distancesSquared[points[index0]] + 1 >= sqrdMaxedDist && distancesSquared[points[index2]] + 1 >= sqrdMaxedDist))
                    return 1;

                //Points 0 and 1 are aligned and Points 1 and 2 are aligned
                if (SameEdge(index0, index1) && SameEdge(index1, index2))
                    return 1;

                //Points 0 and 2 are aligned and Points 1 and 2 are on circle edges
                /*if (!SameEdge(index0, index2) && !(distancesSquared[points[index1]] + 1 >= sqrdMaxedDist && distancesSquared[points[index2]] + 1 >= sqrdMaxedDist))
                    return 1;*/

                /*if (distancesSquared[points[index1]] < distancesSquared[points[index0]] && distancesSquared[points[index1]] < distancesSquared[points[index2]])
                    return 1;*/

                if (distancesSquared[points[index0]] + 1 >= sqrdMaxedDist && distancesSquared[points[index1]] + 1 >= sqrdMaxedDist)
                    return 1;

                //Debug.LogUpdate(index0);

                Vector2 a = points[index1];
                points[index1] = points[index2];
                points[index2] = a;
                return 2;
            }

            //CheckAngleAndChange(5, 6, 7);
            //On essaie de résoudre le problème de 2 points ayant le même angle en utilisant le fait que 2 points soit alignés
            for (int i = 0; i < points.Count - 2; i += CheckAngleAndChange(i, i + 1, i + 2))
            { }

            if (points.Count > 1)
            {
                CheckAngleAndChange(points.Count - 1, 0, 1);

                if (points.Count > 2)
                    CheckAngleAndChange(points.Count - 2, points.Count - 1, 0);
            }

            //On vérifie si 2 points sont curved en vérifiant si 2 points sont connectés et sont à la limite du cercle
            PolygonPoint[] polygon = new PolygonPoint[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                //distancesSquared[points[i]] = Vector2.DistanceSquared(middle, points[i]);
                PolygonPoint point = new PolygonPoint(points[i]);
                if (Math.Round(distancesSquared[points[i]], 2) + 1 >= sqrdMaxedDist)
                {
                    if (Math.Round(distancesSquared[points[i + 1 < points.Count ? i + 1 : 0]], 2) + 1 >= sqrdMaxedDist)
                        point.ArchedRight = true;
                    if (Math.Round(distancesSquared[points[i - 1 > 0 ? i - 1 : points.Count - 1]], 2) + 1 >= sqrdMaxedDist)
                        point.ArchedLeft = true;
                }
                polygon[i] = point;
            }

            //Debug

            if (Debug.DebugMode)
            {
                for (int i = 0; i < polygon.Length; i++)
                {
                    Debug.PointUpdate(polygon[i].Position);
                    if (Vector2.DistanceSquared(Input.MousePos, polygon[i].Position) < 1f)
                    {
                        Debug.LogUpdate(i, polygon[i].ArchedLeft, polygon[i].ArchedRight, polygon[i].Position);
                        Debug.LogUpdate("Angle : " + (polygon[i].Position - middle).ToAngle());
                        Debug.LogUpdate("Sqrd Dist : " + distancesSquared[polygon[i].Position]);
                    }
                }
            }

            return polygon;
        }

        public static void DrawCirclePolygon(PolygonPoint[] polygon, Vector2 circleCenter, float circleRadius, Color color, float theta = 0.01f)
        {
            for (int i = 0; i < polygon.Length; i++)
            {
                if (polygon[i].ArchedRight && polygon[i + 1 < polygon.Length ? i + 1 : 0].ArchedLeft)
                {
                    if (!Debug.DebugMode)
                        Drawing.DrawArc(circleCenter, circleRadius, polygon[i].Position, polygon[i + 1 < polygon.Length ? i + 1 : 0].Position, theta, color, 1);
                    else
                        Drawing.DrawLine(polygon[i].Position, polygon[i + 1 < polygon.Length ? i + 1 : 0].Position, new Color(Color.Yellow, 130));
                }
                else
                {
                    if (!Debug.DebugMode)
                        Drawing.DrawLine(polygon[i].Position, polygon[i + 1 < polygon.Length ? i + 1 : 0].Position, color);
                    else
                        Drawing.DrawLine(polygon[i].Position, polygon[i + 1 < polygon.Length ? i + 1 : 0].Position, new Color(Color.Green, 130));
                }
            }

            if (polygon.Length == 0)
                Drawing.DrawCircleEdge(circleCenter, circleRadius, theta, color, 1);
        }
    }

    public class PolygonPoint
    {
        public Vector2 Position;
        public bool ArchedRight;
        public bool ArchedLeft;

        public PolygonPoint(Vector2 position, bool archedRight, bool archedLeft)
        {
            Position = position;
            ArchedRight = archedRight;
            ArchedLeft = archedLeft;
        }

        public PolygonPoint(Vector2 position)
        {
            Position = position;
        }
    }
}
