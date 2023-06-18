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
        public float Direction;
        public float Range;
        public float Length;
        public Color InsideColor;
        public Color OutsideColor;
        
        public ArcLight(Vector2 localPosition, float direction, float range, float length, Color insideColor, Color outsideColor) : base(localPosition, length)
        {
            Direction = MathHelper.ToRadians(direction);
            Range = MathHelper.ToRadians(range);
            Length = length;
            InsideColor = insideColor;
            OutsideColor = outsideColor;
        }

        public ArcLight(Vector2 localPosition, Vector2 direction, float range, float length, Color insideColor, Color outsideColor) : this(localPosition, direction.ToAngleDegrees(), range, length, insideColor, outsideColor) { }

        public override void DrawRenderTarget()
        {
            Vector2 init = RenderTargetPosition + new Vector2(Lighting.MaxLightSize) / 2;
            Drawing.DrawFilledArc(init, Length, Direction - Range / 2, Direction + Range / 2, 0.03f, InsideColor, OutsideColor);
            //Drawing.DrawCircle(init, Length, 0.03f, InsideColor, OutsideColor);
        }
    }
}
