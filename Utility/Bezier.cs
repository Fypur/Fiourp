using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public static class Bezier
    {
        private static List<float[]> binomials = new List<float[]>
        {
            new float[] { 1 },
            new float[] { 1, 1 },
            new float[] { 1, 2, 1 },
            new float[] { 1, 3, 3, 1 },
            new float[] { 1, 4, 6, 4, 1 },
            new float[] { 1, 5, 10, 10, 5, 1 },
            new float[] { 1, 6, 15, 20, 15, 6, 1 }
        };

        public static Vector2 Generic(IList<Vector2> controlPoints, float t)
        {
            if (controlPoints.Count + 1 >= binomials.Count)
                AddBinomials(controlPoints.Count + 1);

            Vector2 sum = Vector2.Zero;

            for (int i = 0; i < controlPoints.Count; i++)
                sum += controlPoints[i] * binomials[controlPoints.Count - 1][i] * (float)Math.Pow(1 - t, controlPoints.Count - 1 - i) * (float)Math.Pow(t, i);

            return sum;
        }

        /// <summary>
        /// Uses 3 Control Points
        /// </summary>
        /// <param name="controlPoints"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector2 Quadratic(IList<Vector2> controlPoints, float t)
            => (1 - t) * (1 - t) * controlPoints[0] + 2 * (1 - t) * t * controlPoints[1] + t * t * controlPoints[2];

        /// <summary>
        /// Uses 4 Control Points
        /// </summary>
        /// <param name="controlPoints"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector2 Cubic(IList<Vector2> controlPoints, float t)
            => (1 - t) * (1 - t) * (1 - t) * controlPoints[0] + 3 * (1 - t) * (1 - t) * t * controlPoints[1] + 3 * (1 - t) * t * t * controlPoints[2] + t * t * t * controlPoints[3];

        private static void AddBinomials(int untilDepth)
        {
            while (untilDepth >= binomials.Count)
            {
                float[] next = new float[binomials.Count + 1];

                next[0] = 1;

                for (int i = 1; i < next.Length - 1; i++)
                    next[i] = binomials[binomials.Count - 1][i - 1] + binomials[binomials.Count - 1][i];

                next[next.Length - 1] = 1;

                binomials.Add(next);
            }
        }
    }
}
