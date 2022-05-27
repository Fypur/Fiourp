using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class BoolSwitcher : Switcher
    {
        public bool CurrentValue { get => CurrentIndex == 2; set => CurrentIndex = value ? 1 : 0; }
        public BoolSwitcher(Vector2 position, int width, int height, bool centered, string fieldName, bool startValue, Action onOn, Action onOff) : base(position, width, height, centered, fieldName, startValue ? 2 : 1, 4, new() { { 0, onOn }, { 1, onOff }, { 2, onOn }, { 3, onOff } })
        {
            valueTextBox.Text = startValue ? "On" : "Off";
        }

        public override void Update()
        {
            base.Update();
            if (CurrentIndex == 0)
            {
                CurrentIndex = 2;
                OnMove();
            }
            else if (CurrentIndex == 3)
            {
                CurrentIndex = 1;
                OnMove();
            }
        }

        public override void OnMove()
        {
            valueTextBox.Text = CurrentValue ? "On" : "Off";
        }
    }
}
