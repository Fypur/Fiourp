using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class Switcher : UIElement
    {
        public int CurrentIndex { get; protected set; }
        public int MinValue = 0;
        public int MaxValue;

        public TextBox FieldTextBox;
        protected TextBox ValueTextBox;

        protected Dictionary<int, Action> Actions;
        protected Action<int> Action;
        

        public Switcher(Vector2 position, int width, int height, bool centered, string fieldName, int startValue, int numValues, Dictionary<int, Action> actions) : base(position, width, height, centered, new Sprite(Color.White))
        {
            CurrentIndex = startValue;
            MaxValue = numValues;
            this.Actions = actions;
            
            FieldTextBox = (TextBox)AddChild(new TextBox(fieldName, "LexendDeca", Pos, width / 2, height, 1, Color.Black, true));
            ValueTextBox = (TextBox)AddChild(new TextBox(CurrentIndex.ToString(), "LexendDeca", Pos + HalfSize.OnlyX(), width / 2, height, 1, Color.Black, true));
        }

        public Switcher(Vector2 position, int width, int height, bool centered, string fieldName, int startValue, int minValue, int maxValue, Action<int> action) : base(position, width, height, centered, new Sprite(Color.White))
        {
            CurrentIndex = startValue;
            MinValue = minValue;
            MaxValue = maxValue;
            Action = action;

            FieldTextBox = (TextBox)AddChild(new TextBox(fieldName, "LexendDeca", Pos, width / 2, height, 1, Color.Black, true));
            ValueTextBox = (TextBox)AddChild(new TextBox(CurrentIndex.ToString(), "LexendDeca", Pos + HalfSize.OnlyX(), width / 2, height, 1, Color.Black, true));
        }

        public override void Update()
        {
            base.Update();

            if (!Selected)
                return;

            if (Input.LeftControls.IsDown())
                GoLeft();
            if(Input.RightControls.IsDown())
                GoRight();
        }

        public virtual void GoLeft()
        {
            if (CurrentIndex <= MinValue)
            {
                NotPossible();
                return;
            }

            CurrentIndex--;
            ValueTextBox.SetText(CurrentIndex.ToString());
            if (Actions != null && Actions.ContainsKey(CurrentIndex))
                Actions[CurrentIndex]?.Invoke();
            Action?.Invoke(CurrentIndex);
            OnMove();
        }

        public virtual void GoRight()
        {
            if (CurrentIndex >= MaxValue - 1)
            {
                NotPossible();
                return;
            }

            CurrentIndex++;
            ValueTextBox.SetText(CurrentIndex.ToString());
            if (Actions != null && Actions.ContainsKey(CurrentIndex))
                Actions[CurrentIndex]?.Invoke();
            Action?.Invoke(CurrentIndex);
            OnMove();
        }

        public virtual void OnMove() { }

        public virtual void NotPossible()
        {
            if (!HasComponent<Shaker>())
            {
                AddComponent(new Shaker(0.2f, (Size.X > Size.Y ? Size.X : Size.Y) * 0.005f, null, true));
            }
        }

        public override void OnSizeChange()
        {
            base.OnSizeChange();
            //RemoveComponent<Shaker>();
        }
    }
}
