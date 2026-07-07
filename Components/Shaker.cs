using Microsoft.Xna.Framework;
using System.Collections;

namespace Fiourp
{
    public class Shaker : Component
    {
        public float Intensity;

        public Sprite ShakeSprite;
        public float Time;
        private float timeMaxValue;

        private Coroutine shakeCoroutine;

        public Shaker(float time, float intensity, Sprite shakeSprite = null)
        {
            Time = time;
            timeMaxValue = Time;
            Intensity = intensity;
            ShakeSprite = shakeSprite;
        }

        public override void Added()
        {
            ParentEntity.AddComponent(shakeCoroutine = new Coroutine(Shake()));
        }

        //This requires a full rewrite to make it more solid, and not need the movingPos function (add vectors and keep in memory the fully added vector, then remove it at the end)
        private IEnumerator Shake()
        {
            Vector2 addedShake = Vector2.Zero;


            while (Time > 0)
            {
                Vector2 current = ShakeSprite == null ? ParentEntity.Pos : ShakeSprite.Offset;
                Vector2 initPos = current - addedShake;
                Vector2 random = new Vector2(Rand.NextFloat(-1, 1), Rand.NextFloat(-1, 1)) * Intensity;
                random = Vector2.Clamp(current + random, initPos - new Vector2(Intensity), initPos + new Vector2(Intensity)) - current;

                if (ShakeSprite != null)
                {
                    ShakeSprite.Offset += random;
                    addedShake += random;
                }
                else
                    addedShake += ParentEntity.Move(random);

                Time -= Engine.Deltatime;
                yield return 0;
            }

            if (ShakeSprite != null)
                ShakeSprite.Offset -= addedShake;
            else
                ParentEntity.Move(-addedShake);

            SelfDestroy();
        }


        public override void Removed()
        {
            base.Removed();

            if (shakeCoroutine != null && ParentEntity.HasComponent(shakeCoroutine))
                shakeCoroutine.SelfDestroy();
        }
    }
}
