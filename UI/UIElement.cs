using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public abstract class UIElement : Entity
    {
        public UIElement(Vector2 position, int width, int height, Sprite sprite) : base(position, width, height, sprite)
        {
            RemoveComponent(Collider);
        }
    }
}
