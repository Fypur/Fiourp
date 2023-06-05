using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class BezierCurveRenderer : Renderer
    {
        public List<Vector2> ControlPoints;
        public int Thickness;
        public Color Color;

        public BezierCurveRenderer(Color color, int thickness, params Vector2[] controlPoints)
        {
            ControlPoints = new List<Vector2>(controlPoints);
            Thickness = thickness;
            Color = color;
        }

        public override void Render()
        {
            base.Render();

            float subdivs = 0;
            for (int i = 0; i < ControlPoints.Count - 1; i++)
                subdivs += Vector2.Distance(ControlPoints[i], ControlPoints[i + 1]);

            subdivs /= 10;
            subdivs = (float)Math.Ceiling(HyperBolic(subdivs));


            //Debug.PointUpdate(Color.Blue, ControlPoints.ToArray());
            for (float t = 0; t <= 1 - 0.5f/subdivs; t += 1 / subdivs)
            {
                Drawing.DrawLine(Bezier.Generic(ControlPoints, t), Bezier.Generic(ControlPoints, t + 1 / subdivs), Color, Thickness);
            }
        }

        private static float HyperBolic(float x)
            => 1 + (float)Math.Sqrt((x - 1) * (x - 1) + 9);
    }
}
