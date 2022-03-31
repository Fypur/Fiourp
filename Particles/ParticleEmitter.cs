using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp.Particles
{
    public class ParticleEmitter : Component
    {
        public ParticleSystem ParticleSystem;
        public ParticleType ParticleType;
        public Vector2 LocalPosition;
        public int Amount;
        private float timer;

        public ParticleEmitter(ParticleSystem particleSystem, ParticleType particleType, Vector2 localPosition, float timer, int amount = 1) : base()
        {
            this.timer = timer;
            Amount = amount;
        }

        public override void Update()
        {
            timer -= Engine.Deltatime;

            if(timer <= 0)
            {
                Destroy();
                return;
            }

            Emit();
        }

        public void Emit()
        {
            ParticleSystem.Emit(ParticleType, ParentEntity.Pos + LocalPosition, ParentEntity, Amount);
        }
    }
}
