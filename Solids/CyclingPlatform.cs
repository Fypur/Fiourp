using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class CyclingPlatform : CyclingSolid
    {
        public Color Color;

        public CyclingPlatform(int width, int height, Color color, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction) :
            base(width, height, color, positions, timesBetweenPositions, easingfunction) { }
    }
}
