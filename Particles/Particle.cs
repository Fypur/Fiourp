using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class Particle : Entity
    {
        public Entity Followed;
        public ParticleType Type;

        public float LifeTime;

        public float StartLifeTime;
        public Color StartColor;

        public Vector2 Velocity;
        public float StartSize;

        public Color Color => Sprite.Color;

        public Action<Particle> CustomUpdate;
        public Action<Particle> CustomRender;

        public Particle(ParticleType type, Entity followed, Vector2 position, float Size) : base(position, (int)Size, (int)Size, new Sprite(type.Texture))
        {
            Type = type;
            Followed = followed;
            LifeTime = StartLifeTime;
            StartSize = Size;
            Collider.DebugDraw = false;
            Collider.DebugColor = Color.LightBlue;
        }

        public override void Update()
        {
            LifeTime -= Engine.Deltatime;

            if(LifeTime <= 0)
            {
                Active = false;
                Visible = false;
                return;
            }

            Velocity += Type.Acceleration * Engine.Deltatime;
            Velocity -= Velocity * Type.Friction * Engine.Deltatime;
            Velocity *= (float)Math.Pow(Type.SpeedMultiplier, Engine.Deltatime);
            Pos += Velocity * Engine.Deltatime;

            if(Type.Color2 != null)
                Sprite.Color = Color.Lerp(StartColor, Type.Color2.Value, Ease.Reverse(LifeTime / StartLifeTime));

            switch (Type.FadeMode)
            {
                case ParticleType.FadeModes.Linear:
                    Sprite.Color.A = (byte)(LifeTime / StartLifeTime * StartColor.A);
                    break;
                case ParticleType.FadeModes.EndLinear:
                    if (LifeTime <= StartLifeTime * 0.25f)
                        Sprite.Color.A = (byte)(LifeTime / (StartLifeTime * 0.25f) * StartColor.A);
                    break;
                case ParticleType.FadeModes.Smooth:
                    Sprite.Color.A = (byte)Ease.QuintInAndOut(LifeTime / (StartLifeTime * 0.25f) * StartColor.A);
                    break;
                case ParticleType.FadeModes.EndSmooth:
                    if (LifeTime <= StartLifeTime * 0.25f)
                        Sprite.Color.A = (byte)Ease.QuintInAndOut(LifeTime / (StartLifeTime * 0.25f) * StartColor.A);
                    break;
                default:
                    break;
            }

            switch (Type.SizeChange)
            {
                case ParticleType.FadeModes.Linear:
                    Size = Vector2.One * StartSize * (LifeTime / StartLifeTime);
                    break;
                case ParticleType.FadeModes.EndLinear:
                    if(LifeTime <= StartLifeTime * 0.25f)
                        Size = Vector2.One * StartSize * (LifeTime / (StartLifeTime * 0.25f));
                    break;
                case ParticleType.FadeModes.Smooth:
                    Size = Vector2.One * StartSize * Ease.QuintOut(LifeTime / StartLifeTime);
                    break;
                case ParticleType.FadeModes.EndSmooth:
                    if (LifeTime <= StartLifeTime * 0.25f)
                        Size = Vector2.One * StartSize * Ease.QuintInAndOut(LifeTime / (StartLifeTime * 0.25f));
                    break;
                default:
                    break;
            }

            CustomUpdate?.Invoke(this);
        }

        public override void Render()
        {
            if(Followed != null)
                Sprite.Offset = Followed.Pos;

            base.Render();

            CustomRender?.Invoke(this);
        }

        public override string ToString()
        {
            return $"Particle: Type: {Type}, Followed: {Followed},\n Current LifeTime: {LifeTime}, Start Life Time: {StartLifeTime},\n Start Color: {StartColor},\n Velocity: {Velocity},\n StartSize: {StartSize}, Size: {Size},\n Sprite: {Sprite}";
        }
    }
}
