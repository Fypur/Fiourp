using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class CyclingPlatform : CyclingSolid
    {
        public Color Color;

        public CyclingPlatform(Vector2 position, int width, int height, Sprite sprite, bool goingForwards, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction) :
            base(position, width, height, sprite, goingForwards, positions, timesBetweenPositions, easingfunction)
        { }

        public CyclingPlatform(int width, int height, Sprite sprite, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction) :
            base(width, height, sprite, positions, timesBetweenPositions, easingfunction) { }
    }
}
