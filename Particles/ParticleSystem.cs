using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class ParticleSystem
    {
        private static int maxParticles = 3000;
        public float LayerDepth = 0;

        private int index = 0;
        public Particle[] Particles = new Particle[maxParticles];

        public ParticleSystem()
        { }


        /*public void AddParticles(int particleNumber, float lifetime, Vector2 minParticleSize, Vector2 maxParticleSize, Color originColor, Color towardsColor, Sprite[] particleSprites, float minTime, float maxTime, Vector2 initVelocity)
        {
            for (int i = 0; i < particleNumber; i++)
            {
                AddComponent(new Timer(NextFloat(random, minTime, maxTime), true, null, () =>
                {
                    Vector2 pos = Pos + new Vector2(Width * (float)random.NextDouble(), Height * (float)(random.NextDouble()));
                    Vector2 size = random.VectorBetween(minParticleSize, maxParticleSize);
                    Sprite sprite = particleSprites[random.Next(particleSprites.Length - 1)].Copy();
                    sprite.Color = originColor;

                    Particle p = new Particle(this, pos, (int)size.X, (int)size.Y, lifetime, initVelocity, sprite);
                    p.TowardsColor = towardsColor;
                
                Particles.Add(p);
                    
                }));
            }
        }*/

        public void Emit(Particle particle)
        {
            Particles[index] = particle;
            index++;

            /*{
                maxParticles = 3000;
                Particles = new Particle[maxParticles];
            }*/
            

            if (index >= maxParticles)
                index = 0;
        }

        public void Emit(Particle particle, int amount)
        {
            for (int i = 0; i < amount; i++)
                Emit(particle);
        }

        public void Emit(ParticleType particle, int amount, Vector2 position, Entity followed, float direction, Color color)
        {
            for(int i = 0; i < amount; i++)
                Emit(particle.Create(followed, position, direction, color));
        }

        public void Emit(ParticleType particle, Rectangle rectangle, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector2 position = new Vector2(Rand.NextFloat(rectangle.X, rectangle.Right - particle.Size / 2), Rand.NextFloat(rectangle.Y, rectangle.Bottom - particle.Size / 2));
                Emit(particle.Create(position));
            }
        }

        public void Emit(ParticleType particle, Vector2 lineBegin, Vector2 lineEnd, int amount, float direction)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector2 position = lineBegin + (lineEnd - lineBegin) * Rand.NextDouble();
                Emit(particle.Create(null, position, direction));
            }
        }

        public void Emit(ParticleType particle, int amount, Rectangle rectangle, Entity followed, float direction, Color color)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector2 position = new Vector2(Rand.NextFloat(rectangle.X, rectangle.Right - particle.Size / 2), Rand.NextFloat(rectangle.Y, rectangle.Bottom - particle.Size / 2));
                Emit(particle.Create(followed, position, direction, color));
            }
        }

        public void Emit(ParticleType particle, Vector2 position, int amount = 1)
        {
            for (int i = 0; i < amount; i++)
                Emit(particle.Create(position));
        }

        public void Emit(ParticleType particle, Vector2 position, Entity followed, int amount = 1)
        {
            for (int i = 0; i < amount; i++)
                Emit(particle.Create(followed, position));
        }

        public void Emit(ParticleType particle, Rectangle rectangle, Entity followed, int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector2 position = new Vector2(Rand.NextFloat(rectangle.X, rectangle.Right - particle.Size / 2), Rand.NextFloat(rectangle.Y, rectangle.Bottom - particle.Size / 2));
                Emit(particle.Create(followed, position));
            }
        }

        public void Update()
        {
            for (int i = Particles.Length - 1; i >= 0; i--)
                if (Particles[i] != null && Particles[i].Active)
                    Particles[i].Update();
                else
                    Particles[i] = null;
        }

        public void Render()
        {
            foreach(Particle particle in Particles)
                if(particle != null && particle.Visible)
                    particle.Render();
        }
    }
}
