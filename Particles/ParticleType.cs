using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class ParticleType
    {
        public Texture2D Texture;

        public Color Color;
        public Color? Color2;

        public float SpeedMin;
        public float SpeedMax;
        public float SpeedMultiplier;

        public float LifeMin;
        public float LifeMax;

        public Vector2 Acceleration;
        public float Friction;

        public float Size;
        public float SizeRange;
        public FadeModes SizeChange;

        public float Direction;
        public float DirectionRange;

        public FadeModes FadeMode;

        public enum FadeModes { None, Linear, Smooth, EndLinear, EndSmooth };

        private static Random random = new Random();

        public ParticleType()
        {
            Texture = Drawing.pointTexture;

            Color = Color.White;
            Color2 = null;

            SpeedMin = SpeedMax = 0;
            SpeedMultiplier = 1;

            LifeMin = LifeMax = 0;

            Acceleration = Vector2.Zero;
            Friction = 0;

            Size = 2f;
            SizeRange = 0f;

            SizeChange = FadeModes.None;
            FadeMode = FadeModes.None;
        }

        public Particle Create(Entity followed, Vector2 position)
            => Create(followed, position, Direction, Color);

        public Particle Create(Entity followed, Vector2 position, float direction)
            => Create(followed, position, direction, Color);

        public Particle Create(Vector2 position)
            => Create(null, position, Direction, Color);

        public Particle Create(Entity followed, Vector2 position, float direction, Color color)
        {
            Particle p = new Particle(this, followed, position, Size - 0.5f * SizeRange + (float)random.NextDouble() * SizeRange);

            p.Followed = followed;
            p.StartLifeTime = p.LifeTime = (float)random.NextDouble() * (LifeMax - LifeMin) + LifeMin;
            p.StartColor = p.Sprite.Color = color;

            float dir = direction - 0.5f * DirectionRange + (float)random.NextDouble() * DirectionRange;
            p.Velocity = VectorHelper.AngleToVector(dir) * ((float)random.NextDouble() * (SpeedMax - SpeedMin) + SpeedMin);

            return p;
        }
    }
}
