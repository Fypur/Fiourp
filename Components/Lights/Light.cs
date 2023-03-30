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
        public Vector2 LocalPosition;
        public Vector2 WorldPosition => ParentEntity.Pos + LocalPosition;

        public Vector2 RenderTargetPosition;

        public static readonly Color TransparentWhite = new Color(Color.White, 0);

        public Light(Vector2 localPosition)
        {
            LocalPosition = localPosition;
        }

        public override void Render()
        {
            Lighting.DrawLight(this);
        }

        public abstract void DrawRenderTarget();

        public virtual bool CheckSize()
            => true;
    }
}
