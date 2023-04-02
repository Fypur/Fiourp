using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class PointLight : Light
    {
        public Vector2 Direction;
        public float Range;
        public Color InsideColor;
        public Color OutsideColor;
        public PointLight(Vector2 localPosition, Vector2 direction, float length, Color insideColor, Color outsideColor,float range) : base(localPosition, length)
        {
            Direction = direction;
            InsideColor = insideColor;
            OutsideColor = outsideColor;
            Range = range;
        }

        public override void DrawRenderTarget()
        {
            Vector2 init = RenderTargetPosition + new Vector2(Lighting.MaxLightSize) / 2;
            Drawing.DrawTriangle(init, InsideColor, init + VectorHelper.RotateDeg(Direction * Size, -Range), OutsideColor, init + VectorHelper.RotateDeg(Direction * Size, Range), OutsideColor);
        }
    }
}
