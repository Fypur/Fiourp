using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class ArcLight : Light
    {
        public Vector2 Direction;
        public float Range;
        public Color InsideColor;
        public Color OutsideColor;
        public ArcLight(Vector2 localPosition, Vector2 direction, float length, Color insideColor, Color outsideColor,float range) : base(localPosition, length)
        {
            Direction = direction;
            InsideColor = insideColor;
            OutsideColor = outsideColor;
            Range = range;
        }

        public override void DrawRenderTarget()
        {
            Vector2 init = RenderTargetPosition + new Vector2(Lighting.MaxLightSize) / 2;
            Drawing.DrawFilledArc(init, Range, init + VectorHelper.RotateDeg(Direction * Size, -Range), init + VectorHelper.RotateDeg(Direction * Size, Range), 0.03f, InsideColor, OutsideColor);
        }
    }
}
