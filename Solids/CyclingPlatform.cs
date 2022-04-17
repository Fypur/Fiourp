using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class CyclingPlatform : CyclingSolid
    {
        public Color Color;

        public CyclingPlatform(Vector2 position, int width, int height, Color color, bool goingForwards, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction) :
            base(position, width, height, new Sprite(color), goingForwards, positions, timesBetweenPositions, easingfunction)
        { }

        public CyclingPlatform(int width, int height, Color color, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction) :
            base(width, height, new Sprite(color), positions, timesBetweenPositions, easingfunction) { }
    }
}
