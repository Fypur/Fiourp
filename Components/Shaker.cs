using Microsoft.Xna.Framework;
using System;
using System.Collections;

namespace Fiourp
{
    public class Shaker : Component
    {
        public float Intensity;
        public Func<Vector2> UpdatedInitPos;

        public Sprite ShakeSprite;
        public float Time;
        public bool DestroyOnEnd;
        private float timeMaxValue;

        private Vector2 initPos;
        private Coroutine shakeCoroutine;

        public Shaker(float time, float intensity, Func<Vector2> movingPos = null, Sprite shakeSprite = null)
        {
            Time = time;
            timeMaxValue = Time;
            Intensity = intensity;
            ShakeSprite = shakeSprite;
            UpdatedInitPos = movingPos;
        }

        public override void Added()
        {
            initPos = ParentEntity.Pos;

            if (ShakeSprite != null)
                initPos = ShakeSprite.Offset;

            ParentEntity.AddComponent(shakeCoroutine = new Coroutine(Shake()));
        }

        //This requires a full rewrite to make it more solid, and not need the movingPos function (add vectors and keep in memory the fully added vector, then remove it at the end)
        private IEnumerator Shake()
        {
            if (ShakeSprite != null)
            {
                void MoveSpriteBy(Entity entity, Vector2 offset)
                {
                    if (ShakeSprite != null)


                }

                while (Time > 0)
                {
                    initPos = UpdatedInitPos == null ? initPos : UpdatedInitPos();
                    Vector2 random = new Vector2(Rand.NextFloat(-1, 1), Rand.NextFloat(-1, 1)) * Intensity;
                    random = Vector2.Clamp(ShakeSprite.Offset + random, initPos - new Vector2(Intensity), initPos + new Vector2(Intensity)) - ShakeSprite.Offset;

                    ShakeSprite.Offset += random;

                    Time -= Engine.Deltatime;
                    yield return 0;
                }

                ShakeSprite.Offset += initPos - ShakeSprite.Offset;

                if (DestroyOnEnd)
                    Destroy();
            }
            else
            {
                while (Time > 0)
                {
                    initPos = UpdatedInitPos == null ? initPos : UpdatedInitPos();
                    Vector2 random = new Vector2(Rand.NextFloat(-1, 1), Rand.NextFloat(-1, 1)) * Intensity;
                    random = Vector2.Clamp(ParentEntity.Pos + random, initPos - new Vector2(Intensity) * (Time / timeMaxValue), initPos + new Vector2(Intensity) * (Time / timeMaxValue)) - ParentEntity.Pos;

                    ParentEntity.Move(random);
                    Time -= Engine.Deltatime;
                    yield return 0;
                }

                ParentEntity.Move(initPos - ParentEntity.Pos);

                Destroy();
            }
        }

        public override void Removed()
        {
            base.Removed();

            if (shakeCoroutine != null && ParentEntity.Components.Contains(shakeCoroutine))
                shakeCoroutine.Destroy();
        }
    }
}
