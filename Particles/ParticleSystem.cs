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
        public float SpeedMultiplier = 1;
        public Color? TowardsColor = null;

        public bool DestroyOnEnd;

        public List<Particle> Particles = new();

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
            => Particles.Add(particle);

        public void Emit(Particle particle, int amount)
        {
            for (int i = 0; i < amount; i++)
                Particles.Add(particle);
        }

        public void Emit(ParticleType particle, Vector2 position)
            => Particles.Add(particle.Create(position));

        public void Emit(ParticleType particle, int amount, Vector2 position, Entity followed, float direction, Color color)
        {
            for(int i = 0; i < amount; i++)
                Particles.Add(particle.Create(followed, position, direction, color));
        }

        public void Emit(ParticleType particle, int amount, Rectangle rectangle, Entity followed, float direction, Color color)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector2 position = new Vector2(Rand.NextFloat(rectangle.X, rectangle.Right), Rand.NextFloat(rectangle.Y, rectangle.Bottom));
                Particles.Add(particle.Create(followed, position, direction, color));
            }
        }

        public void Emit(ParticleType particle, Vector2 position, int amount = 1)
        {
            for (int i = 0; i < amount; i++)
                Particles.Add(particle.Create(position));
        }

        public void Emit(ParticleType particle, Vector2 position, Entity followed, int amount = 1)
        {
            for (int i = 0; i < amount; i++)
                Particles.Add(particle.Create(followed, position));
        }

        public void Update()
        {
            for (int i = Particles.Count - 1; i >= 0; i--)
                if (Particles[i].Active)
                    Particles[i].Update();
                else
                    Particles.RemoveAt(i);
        }

        public void Render()
        {
            for (int i = Particles.Count - 1; i >= 0; i--)
                if(Particles[i].Active)
                    Particles[i].Render();
                else
                    Particles.RemoveAt(i);
        }
    }
}
