using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
    }
}
