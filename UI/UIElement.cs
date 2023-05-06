using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class UIElement : Entity
    {
        public bool Overlay;
        public bool Centered;
        protected bool CustomCenter;

        public bool Selected { get; protected set; }
        public UIElement Left, Right, Up, Down;

        protected bool selectableField = true;

        public bool Selectable { get => selectableField; 
            set 
            { 
                if (value != selectableField) 
                {
                    selectableField = value;
                    if (value)
                    {
                        /*if(Up != null)
                            Up.Down = this;
                        if(Down != null)
                            Down.Up = this;
                        if(Left != null)
                            Left.Right = this;
                        if(Right != null)
                            Right.Left = this;*/
                        OnAddSelectable();
                    }
                    else
                    {
                        /*if (Up != null)
                            Up.Down = Down;
                        if (Down != null)
                            Down.Up = Up;
                        if (Left != null)
                            Left.Right = Right;
                        if (Right != null)
                            Right.Left = Left;*/
                        OnRemoveSelectable();
                    }
                }
            }
        }

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
            PreviousExactPos = ExactPos;
        }

        public UIElement(Vector2 position, int width, int height, bool centered, Sprite sprite) : base(position, width, height, sprite)
        {
            RemoveComponent(Collider);
            Centered = centered;
            if (centered)
                CenteredPos = Pos;
            PreviousExactPos = ExactPos;
        }

        public override void Awake()
        {
            base.Awake();

            Pos = Pos / Options.DefaultUISizeMultiplier * Options.CurrentScreenSizeMultiplier;
            PreviousExactPos = ExactPos;
            Size = Size / Options.DefaultUISizeMultiplier * Options.CurrentScreenSizeMultiplier;
        }

        public override void Update()
        {
            base.Update();

            /*if (Selectable && Active && Visible && Bounds.Contains(Input.MousePosNoRenderTarget))
                Selected = true;*/

            if (Selected)
            {
                UIElement newSelected = null;

                if (Input.UpControls.IsDown())
                    newSelected = NextSelected(Direction.Up);
                else if (Input.DownControls.IsDown())
                    newSelected = NextSelected(Direction.Down);
                else if (Input.LeftControls.IsDown())
                    newSelected = NextSelected(Direction.Left);
                else if (Input.RightControls.IsDown())
                    newSelected = NextSelected(Direction.Right);

                if(newSelected != null)
                {
                    newSelected.OnSelected();
                    OnLeaveSelected();
                }
            }
        }

        public UIElement NextSelected(Direction direction)
        {
            UIElement newSelected = null;


            switch (direction)
            {
                case Direction.Up:
                    newSelected = Up;
                    break;
                case Direction.Down:
                    newSelected = Down;
                    break;
                case Direction.Left:
                    newSelected = Left;
                    break;
                case Direction.Right:
                    newSelected = Right;
                    break;
            }

            if (newSelected != null && !newSelected.Selectable)
                newSelected = newSelected.NextSelected(direction);

            return newSelected;
        }

        public virtual void OnSelected()
        {
            AddComponent(new Coroutine(Coroutine.WaitFramesThen(1, () => Selected = true)));
            if(Sprite != null)
                Sprite.Color = new Color(Sprite.Color.ToVector3() - new Color(50, 50, 50).ToVector3());
        }
        public virtual void OnLeaveSelected() 
        {
            Selected = false;
            if (Sprite != null)
                Sprite.Color = new Color(Sprite.Color.ToVector3() + new Color(50, 50, 50).ToVector3());
        }

        public virtual void OnAddSelectable()
        {
            Sprite.Color = new Color(Sprite.Color.ToVector3() + new Color(100, 100, 100).ToVector3());
        }

        public virtual void OnRemoveSelectable()
        {
            Sprite.Color = new Color(Sprite.Color.ToVector3() - new Color(100, 100, 100).ToVector3());
        }

        public virtual void OnSizeChange() { }
        
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
