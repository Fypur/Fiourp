using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public static class VectorHelper
    {
        public static float GetAngle(Vector2 vector)
            => (float)Math.Atan2(vector.Y, vector.X);

        //3 Different methods
        /*public static float GetAngle(Vector2 from, Vector2 to)
            => GetAngle(from) - GetAngle(to);
            => (float)Math.Acos(Vector2.Dot(from, to) / (from.Length() * to.Length()) % Math.PI);
            => (float)Math.Atan2(from.X * to.Y - from.Y * to.X, Vector2.Dot(from, to));
        */

        public static float GetAngle(Vector2 from, Vector2 to)
            => (float)Math.Atan2(from.X * to.Y - from.Y * to.X, Vector2.Dot(from, to));

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

        public static Vector2 ClosestOnSegment(Vector2 from, Vector2 a, Vector2 b)
        {
            Vector2 v = b - a;
            return a + v * MathHelper.Clamp(Vector2.Dot(from - a, v) / v.LengthSquared(), 0f, 1f);
        }
        public static Vector2 AngleToVector(float angle)
            => new Vector2((float)Math.Cos(MathHelper.ToRadians(angle)), (float)Math.Sin(MathHelper.ToRadians(angle)));
    }
}
