using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Fiourp
{
    public abstract class Solid : Platform
    {
        public Solid(Vector2 position, int width, int height, Sprite sprite)
            : base(position, width, height, sprite)
        {
            Collider = new BoxCollider(Vector2.Zero, width, height);
            AddComponent(Collider);
        }

        public Solid(Vector2 position, int width, int height, Color color)
            : base(position, width, height, new Sprite(color))
        {
            Collider = new BoxCollider(Vector2.Zero, width, height);
            AddComponent(Collider);
        }
    }
}