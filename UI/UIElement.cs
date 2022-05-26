using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public abstract class UIElement : Entity
    {
        public bool Overlay;
        public bool Centered;
        protected bool CustomCenter;

        public Vector2 CenteredPos
        {
            get => !Centered || CustomCenter ? Pos : Pos + HalfSize;
            set
            {
                if (!Centered || CustomCenter)
                    Pos = value;
                else
                    Pos = value - HalfSize;
            }
        }

        public new int Width
        {
            get => base.Width;
            set
            {
                if (!Centered || CustomCenter)
                    base.Width = value;
                else
                {
                    Vector2 oldPos = CenteredPos;
                    base.Width = value;
                    CenteredPos = oldPos;
                }
            }
        }

        public new int Height
        {
            get => base.Height;
            set
            {
                if (!Centered || CustomCenter)
                    base.Height = value;
                else
                {
                    Vector2 oldPos = CenteredPos;
                    base.Height = value;
                    CenteredPos = oldPos;
                }
            }
        }

        public UIElement(Vector2 position, int width, int height, Sprite sprite) : base(position, width, height, sprite)
        {
            RemoveComponent(Collider);
            Centered = false;
        }

        public UIElement(Vector2 position, int width, int height, bool centered, Sprite sprite) : base(position, width, height, sprite)
        {
            RemoveComponent(Collider);
            Centered = centered;
            if (centered)
            {
                CenteredPos = position;
            }
        }

        public void AddElements(List<UIElement> uiElements)
        {
            if (uiElements == null)
                return;

            foreach(UIElement element in uiElements)
                AddChild(element);
        }

        public void RemoveElements(List<UIElement> uiElements)
        {
            if (uiElements == null)
                return;

            foreach (UIElement element in uiElements)
                Children.Remove(element);
        }

        public void RemoveAllElements()
            => Children.Clear();
    }
}
