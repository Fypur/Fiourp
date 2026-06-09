using Microsoft.Xna.Framework;
using System;

namespace Fiourp
{
    public class CyclingPlatform : Solid
    {
        public Color Color;

        public CyclingPlatform(Vector2 position, int width, int height, Sprite sprite, bool goingForwards, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction) :
            base(position, new AABBCollider(Vector2.Zero, width, height), sprite)
        {
            AddComponent(new CycleMover(position, width, height, goingForwards, positions, timesBetweenPositions, easingfunction, out Vector2 initPos));
            ExactPos = initPos;
        }

        public CyclingPlatform(int width, int height, Sprite sprite, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction) :
            base(positions[0], new AABBCollider(Vector2.Zero, width, height), sprite)
        {
            AddComponent(new CycleMover(positions, timesBetweenPositions, easingfunction));
        }
    }
}
