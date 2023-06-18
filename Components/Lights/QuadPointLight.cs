using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class QuadPointLight : Light
    {
        public Vector2 Direction;
        public Vector2 QuadPos2;
        public float Range;

        public Color InsideColor;
        public Color OutsideColor;
        
        public QuadPointLight(Vector2 localPosition, Vector2 SecondQuadLocalPosition, float direction, float range, float length, Color insideColor, Color outsideColor) : base(localPosition, length)
        {
            if(Math.Abs(direction) % 360 < 90)
            {
                Vector2 v = LocalPosition;
                LocalPosition = SecondQuadLocalPosition;
                SecondQuadLocalPosition = v;
            }


            Direction = VectorHelper.AngleToVector(direction);
            InsideColor = insideColor;
            OutsideColor = outsideColor;
            Range = range;
            QuadPos2 = SecondQuadLocalPosition - LocalPosition;
        }

        public override void DrawRenderTarget()
        {
            Vector2 init = RenderTargetPosition + new Vector2(Lighting.MaxLightSize) / 2;

            Drawing.DrawQuad(init, InsideColor, init + QuadPos2, InsideColor, init + QuadPos2 + VectorHelper.RotateDeg(Direction * Size, -Range), OutsideColor, init + VectorHelper.RotateDeg(Direction * Size, Range), OutsideColor);
        }

        public override bool OverSize()
            => Size + QuadPos2.Length() > Lighting.MaxLightSize / 2;
    }
}
