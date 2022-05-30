using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class SubMenu : UIElement
    {
        public new List<UIElement> Children;
        public SubMenu Parent;
        public bool Vertical;

        public SubMenu(SubMenu parentSubMenu, Vector2 position, int width, int height, bool vertical) : base(position, width, height, null)
        {
            Parent = parentSubMenu;
            Vertical = vertical;
        }

        public override void Update()
        {
            base.Update();

            if(!Active || !Visible)
                return;

            if (Input.UIActionBack.IsDown())
                Parent.Instantiate();
        }

        public virtual List<UIElement> GetElements() { return new(); }
        public void RefreshElements()
        {
            Children = GetElements();
        }

        public void AddElement(UIElement element)
        {
            Children.Add(element);
            if (Children.Count > 1)
            {
                if (Vertical)
                {
                    Children[0].Down = element;
                    element.Up = Children[0];
                }
                else
                {
                    Children[0].Right = element;
                    element.Left = Children[0];
                }
            }
        }

        public void SwitchTo(SubMenu other)
        {
            SelfDestroy();
            other.Instantiate();
        }

        public void Instantiate()
        {
            RefreshElements();
            bool doOnce = true;
            foreach(UIElement ui in Children)
            {
                if (doOnce && ui.Selectable)
                {
                    ui.Selected = true;
                    doOnce = false;
                }
                Engine.CurrentMap.Instantiate(ui);
            }
        }
    }
}
