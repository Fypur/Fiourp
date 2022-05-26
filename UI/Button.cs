using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class Button : UIElement
    {
        public Action OnClickAction;

        private bool hovered;
        private bool pressed;

        public Button(Vector2 position, int width, int height, bool centered, Sprite sprite, Action onPressed) : base(position, width, height, centered, sprite)
        {
            OnClickAction = onPressed;
            /*Sprite.NineSliceSettings = new NineSliceSettings(DataManager.GetTexture("9Slice/Button/corner"), DataManager.GetTexture("9Slice/Button/top"), Drawing.pointTexture);*/
        }

        public Button(Vector2 position, int width, int height, bool centered, Sprite sprite) : base(position, width, height, centered, sprite)
        { }

        public override void Update()
        {
            base.Update();

            bool mouseIn = Bounds.Contains(Input.ScreenMousePos);

            if (!hovered && mouseIn)
            {
                hovered = true;
                OnHover();
            }
            else if(hovered && !mouseIn)
            {
                hovered = false;
                OnLeaveHover();
                if (pressed)
                {
                    pressed = false;
                    OnLeaveHold();
                }
            }

            if (Input.GetMouseButtonDown(MouseButton.Left) && mouseIn)
            {
                pressed = true;
                OnHold();
            }
            else if (pressed && Input.GetMouseButtonUp(MouseButton.Left))
            {
                if (mouseIn)
                {
                    OnClick();
                    ClickAnimation();
                }

                pressed = false;
                OnLeaveHold();
            }
        }

        public virtual void OnClick() { OnClickAction?.Invoke(); }

        public virtual void ClickAnimation()
        {

        }

        public virtual void OnHover()
        {
            Sprite.Color.R -= 20;
            Sprite.Color.B -= 20;
            Sprite.Color.G -= 20;
        }

        public virtual void OnLeaveHover()
        {
            Sprite.Color.R += 20;
            Sprite.Color.B += 20;
            Sprite.Color.G += 20;
        }

        public virtual void OnHold()
        {
            Sprite.Color.R -= 20;
            Sprite.Color.B -= 20;
            Sprite.Color.G -= 20;
        }

        public virtual void OnLeaveHold()
        {
            Sprite.Color.R += 20;
            Sprite.Color.B += 20;
            Sprite.Color.G += 20;
        }
    }
}
