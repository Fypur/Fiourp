using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class CircleLight : Light
    {
        public float Radius;
        public Color InsideColor;
        public Color OutsideColor;

        public CircleLight(Vector2 localPosition, float radius, Color insideColor, Color outsideColor) : base(localPosition, radius)
        {
            Radius = radius;
            InsideColor = insideColor;
            OutsideColor = outsideColor;
        }

        public override void DrawRenderTarget()
        {
            Drawing.DrawCircle(RenderTargetPosition + new Vector2(Lighting.MaxLightSize) / 2, Radius, 0.1f, InsideColor, OutsideColor);
        }
    }
}
