using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class ParticleEmitter : Component
    {
        public ParticleSystem ParticleSystem;
        public ParticleType ParticleType;
        public Vector2 LocalPosition;
        public Rectangle LocalBounds;

        public int Amount;
        public float? Direction;
        public Color Color = Color.White;

        private float timer = float.NaN;

        public ParticleEmitter(ParticleSystem particleSystem, ParticleType particleType, Vector2 localPosition, int amount) : base()
        {
            ParticleSystem = particleSystem;
            ParticleType = particleType;
            LocalPosition = localPosition;
            Amount = amount;
        }

        public ParticleEmitter(ParticleSystem particleSystem, ParticleType particleType, Vector2 localPosition, int amount, float time) : base()
        {
            ParticleSystem = particleSystem;
            ParticleType = particleType;
            LocalPosition = localPosition;
            Amount = amount;
            timer = time;
        }

        public ParticleEmitter(ParticleSystem particleSystem, ParticleType particleType, Rectangle localBounds, int amount) : base()
        {
            ParticleSystem = particleSystem;
            ParticleType = particleType;
            LocalBounds = localBounds;
            Amount = amount;
        }

        public ParticleEmitter(ParticleSystem particleSystem, ParticleType particleType, Vector2 localBounds, int amount, float? direction, Color color) : base()
        {
            ParticleSystem = particleSystem;
            ParticleType = particleType;
            LocalPosition = localBounds;
            Amount = amount;
            Direction = direction;
            Color = color;
        }

        public ParticleEmitter(ParticleSystem particleSystem, ParticleType particleType, Rectangle localBounds, int amount, float time) : base()
        {
            ParticleSystem = particleSystem;
            ParticleType = particleType;
            LocalBounds = localBounds;
            Amount = amount;
            timer = time;
        }

        public override void Added()
        {
            if (timer != float.NaN)
                ParentEntity.AddComponent(new Timer(timer, true, null, () => Destroy()));
        }

        public override void Update()
        {
            if(LocalBounds != Rectangle.Empty)
                ParticleSystem.Emit(ParticleType, new Rectangle(ParentEntity.Pos.ToPoint() + LocalBounds.Location, LocalBounds.Size), Amount);
            else
                ParticleSystem.Emit(ParticleType, Amount, ParentEntity.Pos + LocalPosition, null, Direction.HasValue ? Direction.Value : ParticleType.Direction, Color);
        }

        public void Emit()
        {
            ParticleSystem.Emit(ParticleType, ParentEntity.Pos + LocalPosition, ParentEntity, Amount);
        }
    }
}
