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
        public int MaxValue;

        protected TextBox fieldTextBox;
        public TextBox valueTextBox;
        protected Dictionary<int, Action> actions;

        public Switcher(Vector2 position, int width, int height, bool centered, string fieldName, int startValue, int numValues, Dictionary<int, Action> actions) : base(position, width, height, centered, new Sprite(Color.White))
        {
            CurrentIndex = startValue;
            MaxValue = numValues;
            this.actions = actions;
            
            fieldTextBox = (TextBox)AddChild(new TextBox(fieldName, "LexendDeca", Pos, width / 2, height, Color.Black, 1, true));
            valueTextBox = (TextBox)AddChild(new TextBox(CurrentIndex.ToString(), "LexendDeca", Pos + HalfSize.OnlyX(), width / 2, height, Color.Black, 1, true));
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
            if (CurrentIndex <= 0)
            {
                NotPossible();
                return;
            }

            CurrentIndex--;
            valueTextBox.Text = CurrentIndex.ToString();
            if (actions != null && actions.ContainsKey(CurrentIndex))
                actions[CurrentIndex]?.Invoke();
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
            valueTextBox.Text = CurrentIndex.ToString();
            if (actions != null && actions.ContainsKey(CurrentIndex))
                actions[CurrentIndex]?.Invoke();
            OnMove();
        }

        public virtual void OnMove() { }

        public virtual void NotPossible()
        {
            //TODO: Change this to shaking or smth
            AddComponent(new Shaker(0.2f, 0.2f, null, false));
        }
    }
}
