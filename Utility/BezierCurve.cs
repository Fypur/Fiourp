using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class BezierCurve : Renderer
    {
        public List<Vector2> ControlPoints;
        public int Thickness;
        public Color Color;

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

        public BezierCurve(Color color, int thickness, params Vector2[] controlPoints)
        {
            ControlPoints = new List<Vector2>(controlPoints);
            Thickness = thickness;
            Color = color;
        }

        public override void Render()
        {
            base.Render();

            /*for(float t = 0; t <= 1; t += 0.1f)
            {
                for(int i = 0; i < ControlPoints.Length - 1; i++)
                {
                    for (int j = 0; j < ControlPoints.Length - 1; j++)
                    {
                        Vector2 intermediate = ControlPoints[j] * (ControlPoints[j + 1] - ControlPoints[j]) * t;



                    }
                }

            }*/

            float subdivs = 0;
            for (int i = 0; i < ControlPoints.Count - 1; i++)
                subdivs += Vector2.Distance(ControlPoints[i], ControlPoints[i + 1]);

            subdivs /= 10;
            subdivs = (float)Math.Ceiling(HyperBolic(subdivs));


            //Debug.PointUpdate(Color.Blue, ControlPoints.ToArray());
            for (float t = 0; t <= 1 - 0.5f/subdivs; t += 1 / subdivs)
            {
                Drawing.DrawLine(Generic(t), Generic(t + 1 / subdivs), Color, Thickness);
            }

        }

        private Vector2 Generic(float t)
        {
            if (ControlPoints.Count + 1 >= binomials.Count)
                AddBinomials(ControlPoints.Count + 1);

            Vector2 sum = Vector2.Zero;

            for(int i = 0; i < ControlPoints.Count; i++)
                sum += ControlPoints[i] * binomials[ControlPoints.Count - 1][i] * (float)Math.Pow(1 - t, ControlPoints.Count - 1 - i) * (float)Math.Pow(t, i);

            return sum;
        }

        private Vector2 Quadratic(float t)
            => (1 - t) * (1 - t) * ControlPoints[0] + 2 * (1 - t) * t * ControlPoints[1] + t * t * ControlPoints[2];

        private Vector2 Cubic(float t)
            => (1 - t) * (1 - t) * (1 - t) * ControlPoints[0] + 3 * (1 - t) * (1 - t) * t * ControlPoints[1] + 3 * (1 - t) * t * t * ControlPoints[2] + t * t * t * ControlPoints[3];

        private static void AddBinomials(int untilDepth)
        {
            while(untilDepth >= binomials.Count)
            {
                float[] next = new float[binomials.Count + 1];

                next[0] = 1;

                for (int i = 1; i < next.Length - 1; i++)
                    next[i] = binomials[binomials.Count - 1][i - 1] + binomials[binomials.Count - 1][i];

                next[next.Length - 1] = 1;

                binomials.Add(next);
            }
        }

        private static float HyperBolic(float x)
            => 1 + (float)Math.Sqrt((x - 1) * (x - 1) + 9);
    }
}
