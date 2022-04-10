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
        public bool ShakeSprite;
        public static Random Random = new Random();

        private Vector2 initPos;

        public Shaker(float time, float intensity, bool shakeSprite = false)
        { 
            Time = time;
            Intensity = intensity;
            ShakeSprite = shakeSprite;
        }

        public override void Added()
        {
            initPos = ParentEntity.ExactPos;
            initPos = ParentEntity.Sprite.Offset;
            ParentEntity.AddComponent(new Coroutine(Shake()));
        }

        private IEnumerator Shake()
        {
            if (ShakeSprite)
            {
                while (Time > 0)
                {
                    Vector2 random = new Vector2(RandomFloat(-1, 1), RandomFloat(-1, 1)) * Intensity;
                    random = Vector2.Clamp(ParentEntity.Sprite.Offset + random, initPos - new Vector2(Intensity), initPos + new Vector2(Intensity)) - ParentEntity.Sprite.Offset;

                    ParentEntity.Sprite.Offset += random;

                    Time -= Engine.Deltatime;
                    yield return 0;
                }

                ParentEntity.Sprite.Offset = initPos;
                Destroy();
            }
            else
            {
                while (Time > 0)
                {
                    Vector2 random = new Vector2(RandomFloat(-1, 1), RandomFloat(-1, 1)) * Intensity;
                    random = Vector2.Clamp(ParentEntity.ExactPos + random, initPos - new Vector2(Intensity), initPos + new Vector2(Intensity)) - ParentEntity.ExactPos;

                    if (ParentEntity is MovingSolid s)
                        s.Move(random);
                    else if (ParentEntity is Actor a)
                        a.Move(random);
                    else
                        ParentEntity.Pos += random;

                    Time -= Engine.Deltatime;
                    yield return 0;
                }

                if (ParentEntity is MovingSolid so)
                    so.MoveTo(initPos);
                else if (ParentEntity is Actor ac)
                    ac.MoveTo(initPos);
                else
                    ParentEntity.Pos += initPos;
                Destroy();
            }
        }

        private float RandomFloat(float min, float max)
            => (float)Random.NextDouble() * (max - min) + min;
    }
}
