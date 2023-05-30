using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class TrailRenderer : Renderer
    {
        public static ParticleType Trail;

        public Vector2 LocalPosition;
        public float VelocitySizeMultiplier;

        public Func<bool> Condition;

        public TrailRenderer(Vector2 localPosition, float velocitySizeMultiplier)
        {
            LocalPosition = localPosition;
            VelocitySizeMultiplier = velocitySizeMultiplier;
        }

        public override void Render()
        {
            base.Render();

            if (Condition != null && !Condition())
                return;

            Particle p = Trail.Create(ParentEntity.Pos + LocalPosition);

            float speed;
            if (ParentEntity is Actor actor)
                speed = actor.Velocity.Length();
            else if (ParentEntity is MovingSolid solid)
                speed = solid.Velocity.Length();
            else
                speed = ((ParentEntity.Pos - ParentEntity.PreviousExactPos) / Engine.Deltatime).Length();


            p.StartSize = Trail.Size * speed * VelocitySizeMultiplier;

            int bigSide = ParentEntity.Width > ParentEntity.Height ? ParentEntity.Width : ParentEntity.Height;
            if(p.StartSize > bigSide / 2)
                p.StartSize = bigSide / 2;

            p.Pos -= Microsoft.Xna.Framework.Vector2.One * p.StartSize / 2;

            Engine.CurrentMap.MiddlegroundSystem.Emit(p);
        }
    }
}
