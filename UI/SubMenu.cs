using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class SubMenu : UIElement
    {
        public bool Vertical;
        public bool SelectElementOnOpen = true;

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
            for (int i = 0; i < elements.Count; i++)
            {
                if (i - 1 >= 0)
                {
                    if (vertical)
                        elements[i].Up = elements[i - 1];
                    else
                        elements[i].Left = elements[i - 1];
                }

                if(i + 1 < elements.Count)
                {
                    if (vertical)
                        elements[i].Down = elements[i + 1];
                    else
                        elements[i].Right = elements[i + 1];
                }
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
            Parent.AddChild(subMenu);
            Coroutine c = (Coroutine)AddComponent(new Coroutine(OnClose(), Coroutine.Do(SelfDestroy)));
            //subMenu.Update();
            subMenu.LateUpdate();
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

        public override void Awake()
        {
            RefreshElements();

            Coroutine c = (Coroutine)AddComponent(new Coroutine(OnOpen()));

            if(SelectElementOnOpen)
                SelectFirstElement();


            base.Awake();
        }

        public virtual IEnumerator OnOpen()
        {
            yield return null;
        }

        public virtual IEnumerator OnClose()
        {
            yield return null;
        }

        public IEnumerator Slide(float time, Vector2 offset, List<Entity> entities, bool waitToSelect = false)
        {
            Vector2[] offsets = new Vector2[entities.Count];
            Array.Fill(offsets, offset);

            return Slide(time, offsets, entities, waitToSelect);
        }

        public IEnumerator SlideTo(float time, Vector2 offset, List<Entity> entities, bool waitToSelect = false)
        {
            Vector2[] offsets = new Vector2[entities.Count];
            Array.Fill(offsets, offset);

            return SlideTo(time, offsets, entities, waitToSelect);
        }

        public IEnumerator SlideTo(float time, Vector2[] offsets, List<Entity> entities, bool waitToSelect = false)
        {
            for(int i = 0; i < entities.Count; i++)
            {
                entities[i].Pos += offsets[i];
                offsets[i] = -offsets[i];
            }

            return Slide(time, offsets, entities, waitToSelect);
        }

        public IEnumerator Slide(float time, Vector2[] offsets, List<Entity> entities, bool waitToSelect = false)
        {
            //This bugs out the final position of the UIElement f size is changed through it (finalPos[i] is not changed)
            if (waitToSelect)
                SelectElementOnOpen = false;

            Vector2[] finalPos = entities.Select((e) => e.ExactPos).ToArray();

            float initTime = time;
            float timeOffset = time / finalPos.Length;

            for (int i = 0; i < finalPos.Length; i++)
            {
                entities[i].ExactPos = finalPos[i] + offsets[i] / Options.DefaultUISizeMultiplier * Options.CurrentScreenSizeMultiplier;
            }

            yield return null;

            float t = 0;
            while (t < initTime)
            {
                for (int i = 0; i < finalPos.Length; i++)
                {
                    if (i * timeOffset > t)
                        break;

                    float a = (t - i * timeOffset) / timeOffset;

                    if (a > 1)
                        a = 1;

                    entities[i].ExactPos = Vector2.Lerp(finalPos[i] + offsets[i] / Options.DefaultUISizeMultiplier * Options.CurrentScreenSizeMultiplier, finalPos[i], Ease.QuintInAndOut(a));
                }

                t += Engine.Deltatime;
                yield return null;
            }

            for (int i = 0; i < finalPos.Length; i++)
            {
                entities[i].ExactPos = finalPos[i];
            }

            if(waitToSelect)
                SelectFirstElement();
        }
    }
}
