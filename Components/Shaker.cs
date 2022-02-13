using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class Shaker : Component
    {
        public float Intensity;
        public float Time;

        private Vector2 initPos;

        public Shaker(float time, float intensity)
        { 
            Time = time;
            Intensity = intensity;
        }

        public override void Added()
        {
            initPos = ParentEntity.Pos;
            ParentEntity.AddComponent(new Coroutine(Shake()));
        }

        private IEnumerator Shake()
        {
            while (Time > 0)
            {
                ParentEntity.Pos = initPos + new Vector2(RandomFloat(-1, 1), RandomFloat(-1, 1)) * Intensity;
                Time -= Engine.Deltatime;
                yield return 0;
            }

            Destroy();
        }

        private float RandomFloat(float min, float max)
            => (float)new Random().NextDouble() * (max - min) + min;
    }
}
