using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public static class VectorHelper
    {
        public static float ToAngle(this Vector2 vector)
            => (float)Math.Atan2(vector.Y, vector.X);

        public static float ToAngleDegrees(this Vector2 vector)
            => MathHelper.ToDegrees((float)Math.Atan2(vector.Y, vector.X));

        //3 Different methods
        /*public static float GetAngle(Vector2 from, Vector2 to)
            => GetAngle(from) - GetAngle(to);
            => (float)Math.Acos(Vector2.Dot(from, to) / (from.Length() * to.Length()) % Math.PI);
            => (float)Math.Atan2(from.X * to.Y - from.Y * to.X, Vector2.Dot(from, to));
        */

        public static Vector2 OnlyX(this Vector2 vector)
            => new Vector2(vector.X, 0);

        public static Vector2 OnlyY(this Vector2 vector)
            => new Vector2(0, vector.Y);

        public static float GetAngle(Vector2 from, Vector2 to)
            => (float)Math.Atan2(from.X * to.Y - from.Y * to.X, Vector2.Dot(from, to));

        public static Vector2 Floor(Vector2 value)
            => new Vector2((float)Math.Floor(value.X), (float)Math.Floor(value.Y));

        public static Vector2 Abs(Vector2 vector)
            => new Vector2(Math.Abs(vector.X), Math.Abs(vector.Y));

        /// <summary>
        /// Projection of vector a on vector b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector2 Projection(Vector2 a, Vector2 b)
            => (float)(Vector2.Dot(a, b) / Math.Pow(b.Length(), 2)) * b;

        public static Vector2 ClosestOnSegment(Vector2 point, Vector2 segmentPoint, Vector2 segmentPoint2)
        {
            Vector2 v = segmentPoint2 - segmentPoint;
            return segmentPoint + v * MathHelper.Clamp(Vector2.Dot(point - segmentPoint, v) / v.LengthSquared(), 0f, 1f);
        }

        public static Vector2 AngleToVector(float angle)
            => new Vector2((float)Math.Cos(MathHelper.ToRadians(angle)), (float)Math.Sin(MathHelper.ToRadians(angle)));

        public static Vector2 VectorBetween(this Random random, Vector2 a, Vector2 b)
        {
            float X;
            float Y;

            if (a.X < b.X)
                X = (float)(random.NextDouble() * (b.X - a.X) + a.X);
            else
                X = (float)(random.NextDouble() * (a.X - b.X) + b.X);

            if (a.Y < b.Y)
                Y = (float)(random.NextDouble() * (b.Y - a.Y) + a.Y);
            else
                Y = (float)(random.NextDouble() * (a.Y - b.Y) + b.Y);

            return new Vector2(X, Y);
        }

        public static Vector2 Normalized(this Vector2 vector)
            => Vector2.Normalize(vector);

        public static Vector2 Pow(Vector2 vector, float power)
            => new Vector2((float)Math.Pow(vector.X, power), (float)Math.Pow(vector.Y, power));
    }
}
