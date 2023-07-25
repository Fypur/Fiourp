using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public abstract class Light : Renderer
    {
        public static List<Light> AllLights = new();
        public Vector2 LocalPosition;
        public Vector2 WorldPosition => ParentEntity.Pos + LocalPosition;

        public Vector2 RenderTargetPosition;
        public float Size;

        public static readonly Color TransparentWhite = new Color(Color.White, 0);

        public bool CollideWithWalls = true;

        private Timer blinkTimer;

        public Light(Vector2 localPosition, float size)
        {
            LocalPosition = localPosition;
            Size = size;
        }

        public override void Render()
        {
            Lighting.DrawLight(this);
        }

        public abstract void DrawRenderTarget();

        public virtual bool OverSize()
            => Size > Lighting.MaxLightSize / 2;

        public void StartBlink(float blinkTime)
        {
            blinkTimer = (Timer)ParentEntity.AddComponent(new Timer(blinkTime, false, null, () =>
            {
                Visible = false;
                RefreshBlink(true);
            }));
        }

        private void RefreshBlink(bool visible)
        {
            blinkTimer.Value = blinkTimer.MaxValue;
            blinkTimer.OnComplete =  () =>
            {
                Visible = visible;
                RefreshBlink(!visible);
            };
        }

        public void StopBlink()
        {
            ParentEntity.RemoveComponent(blinkTimer);
        }

        public override void Added()
        {
            base.Added();
            AllLights.Add(this);
        }

        public override void Removed()
        {
            base.Removed();
            AllLights.Remove(this);
        }
    }
}
