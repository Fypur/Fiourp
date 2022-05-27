using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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

        public bool Selected;
        public UIElement Left, Right, Up, Down;
        public bool Selectable = true;

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

        public override void Update()
        {
            base.Update();

            if (Selected)
            {
                UIElement newSelected = null;

                if (Input.UpControls.IsDown() && Up != null && Up.Selectable)
                    newSelected = Up;
                else if (Input.DownControls.IsDown() && Down != null && Down.Selectable)
                    newSelected = Down;
                else if (Input.LeftControls.IsDown() && Left != null && Left.Selectable)
                    newSelected = Left;
                else if (Input.RightControls.IsDown() && Right != null && Right.Selectable)
                    newSelected = Right;

                if(newSelected != null)
                {
                    newSelected.OnSelected();
                    OnLeaveSelected();
                }
            }
        }

        public virtual void OnSelected()
        {
            AddComponent(new Coroutine(Coroutine.WaitFramesThen(1, () => Selected = true)));
            Sprite.Color = new Color(Sprite.Color.ToVector3() - new Color(20, 20, 20).ToVector3());
        }
        public virtual void OnLeaveSelected() 
        {
            Selected = false;
            Sprite.Color = new Color(Sprite.Color.ToVector3() + new Color(20, 20, 20).ToVector3());
        }

        public static void MakeList(List<UIElement> elements, bool vertical)
        {
            int offset = 0;
            for (int i = 0; i < elements.Count; i++)
            {
                if (!elements[i].Selectable)
                {
                    offset++;
                    continue;
                }

                if (i - 1 - offset >= 0)
                {
                    if(vertical)
                        elements[i].Up = elements[i - 1 - offset];
                    else
                        elements[i].Left = elements[i - 1 - offset];
                }
                for(int j = i + 1; j < elements.Count; j++)
                {
                    if (elements[j].Selectable)
                    {
                        if(vertical)
                            elements[i].Down = elements[j];
                        else
                            elements[i].Right = elements[j];
                        i = j - 1;
                        break;
                    }
                }

                offset = 0;
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
                RemoveChild(element);
        }

        public void RemoveAllElements()
            => Children.Clear();
    }
}
