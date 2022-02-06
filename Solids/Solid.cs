using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Fiourp
{
    public abstract class Solid : Entity
    {
        public Solid(Vector2 position, int width, int height, Sprite sprite)
            : base(position, width, height, sprite)
        {

        }

        public Solid(Vector2 position, int width, int height, Color color)
            : base(position, width, height, new Sprite(color))
        {
            Texture2D texture = new Texture2D(Engine.Graphics.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { color });
        }

        public override void Render()
        {
            base.Render();
        }
    }
}