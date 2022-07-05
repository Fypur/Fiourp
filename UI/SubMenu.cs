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
        public bool Vertical;

        public SubMenu(Vector2 position, int width, int height, bool vertical) : base(position, width, height, null)
        {
            Vertical = vertical;
            Overlay = true;
        }

        public override void Update()
        {
            base.Update();

            if(!Active || !Visible)
                return;

            if (Input.UIActionBack.IsDown())
                OnBack();
        }

        public virtual List<UIElement> GetElements() { return new(); }

        public virtual void OnBack() { }

        public void RefreshElements()
        {
            Children.Clear();
            AddChildren(new List<Entity>(GetElements()));
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
                    if (vertical)
                        elements[i].Up = elements[i - 1 - offset];
                    else
                        elements[i].Left = elements[i - 1 - offset];
                }
                for (int j = i + 1; j < elements.Count; j++)
                {
                    if (elements[j].Selectable)
                    {
                        if (vertical)
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

        public void AddElementAtEnd(UIElement element)
        {
            Children.Add(element);
            if (Children.Count > 1)
            {
                UIElement child = (UIElement)Children[Children.Count - 1];
                if (Vertical)
                {
                    child.Down = element;
                    element.Up = child;
                }
                else
                {
                    child.Right = element;
                    element.Left = child;
                }
            }
        }

        public void SwitchTo(SubMenu subMenu)
        {
            SelfDestroy();
            subMenu.Instantiate();
        }

        public void SelectFirstElement()
        {
            foreach (UIElement ui in Children)
            {
                if (ui.Selectable)
                {
                    ui.OnSelected();
                    break;
                }
            }
        }

        public void Instantiate()
        {
            Engine.CurrentMap.Instantiate(this);
            RefreshElements();
            SelectFirstElement();
        }
    }
}
