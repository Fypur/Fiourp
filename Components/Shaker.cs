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
        public Func<Vector2> UpdatedInitPos;

        public bool ShakeSprite;
        public float Time;
        public bool DestroyOnEnd;
        private float timeMaxValue;

        private Vector2 initPos;

        public Shaker(float time, float intensity, Func<Vector2> movingPos = null, bool shakeSprite = false)
        { 
            Time = time;
            timeMaxValue = Time;
            Intensity = intensity;
            ShakeSprite = shakeSprite;
            UpdatedInitPos = movingPos;
        }

        public override void Added()
        {
            initPos = ParentEntity.ExactPos;
            if(ShakeSprite)
                initPos = ParentEntity.Sprite.Offset;
            ParentEntity.AddComponent(new Coroutine(Shake()));
        }

        private IEnumerator Shake()
        {
            if (ShakeSprite)
            {
                void MoveSpriteBy(Entity entity, Vector2 offset)
                {
                    if(entity.Sprite != null)
                        entity.Sprite.Offset += offset;
                    foreach(Entity child in entity.Children)
                        MoveSpriteBy(child, offset);
                }

                while (Time > 0)
                {
                    initPos = UpdatedInitPos == null ? initPos : UpdatedInitPos();
                    Vector2 random = new Vector2(Rand.NextFloat(-1, 1), Rand.NextFloat(-1, 1)) * Intensity;
                    random = Vector2.Clamp(ParentEntity.Sprite.Offset + random, initPos - new Vector2(Intensity), initPos + new Vector2(Intensity)) - ParentEntity.Sprite.Offset;

                    MoveSpriteBy(ParentEntity, random);

                    Time -= Engine.Deltatime;
                    yield return 0;
                }

                MoveSpriteBy(ParentEntity, initPos - ParentEntity.Sprite.Offset);
                //ParentEntity.Sprite.Offset = initPos;

                if(DestroyOnEnd)
                    Destroy();
            }
            else
            {
                void MoveEntityBy(Vector2 move)
                {
                    if (ParentEntity is MovingSolid s)
                        s.Move(move);
                    else if (ParentEntity is Camera cam)
                        cam.NoBoundsPos += move;
                    else if (ParentEntity is Actor a)
                        a.Move(move);
                    else
                        ParentEntity.Pos += move;
                }

                void MoveEntityTo(Vector2 pos)
                {
                    if (ParentEntity is MovingSolid so)
                        so.MoveTo(pos);
                    else if (ParentEntity is Camera cam)
                        cam.NoBoundsPos = pos;
                    else if (ParentEntity is Actor ac)
                        ac.MoveTo(pos);
                    else
                        ParentEntity.Pos = pos;
                }

                while (Time > 0)
                {
                    initPos = UpdatedInitPos == null ? initPos : UpdatedInitPos();
                    Vector2 random = new Vector2(Rand.NextFloat(-1, 1), Rand.NextFloat(-1, 1)) * Intensity;
                    random = Vector2.Clamp(ParentEntity.ExactPos + random, initPos - new Vector2(Intensity) * (Time / timeMaxValue), initPos + new Vector2(Intensity) * (Time / timeMaxValue)) - ParentEntity.ExactPos;

                    MoveEntityBy(random);
                    Time -= Engine.Deltatime;
                    yield return 0;
                }

                MoveEntityTo(initPos);

                Destroy();
            }
        }
    }
}
