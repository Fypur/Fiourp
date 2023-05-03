using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class CyclingPlatform : MovingSolid
    {
        public Color Color;

        public CyclingPlatform(Vector2 position, int width, int height, Sprite sprite, bool goingForwards, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction) :
            base(position, width, height, sprite)
        {
            AddComponent(new CycleMover(position, width, height, goingForwards, positions, timesBetweenPositions, easingfunction, out Vector2 initPos));
            ExactPos = initPos;
        }

        public CyclingPlatform(int width, int height, Sprite sprite, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction) :
            base(positions[0], width, height, sprite)
        {
            AddComponent(new CycleMover(positions, timesBetweenPositions, easingfunction));
        }
    }
}
