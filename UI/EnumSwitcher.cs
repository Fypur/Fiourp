using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class EnumSwitcher<T> : Switcher where T : Enum
    {
        public T CurrentValue { get => Values[CurrentIndex]; set => SetValue(value); }
        private T[] Values;

        public EnumSwitcher(Vector2 position, int width, int height, bool centered, string fieldName, T startValue, Dictionary<T, Action> actions) : base(position, width, height, centered, fieldName, GetParams(startValue, actions, out T[] values, out var intActions), values.Length, intActions)
        {
            Values = values;
            CurrentValue = startValue;
            valueTextBox.Text = CurrentValue.ToString();
        }

        public static int GetParams(T startValue, Dictionary<T, Action> enumActions, out T[] values, out Dictionary<int, Action> intActions)
        {
            values = (T[])Enum.GetValues(typeof(T));
            int startIndex = 0;
            intActions = new();

            for(int i = 0; i < values.Length; i++)
            {
                if(enumActions.TryGetValue(values[i], out Action action))
                    intActions[i] = action;

                if (values[i].Equals(startValue))
                    startIndex = i;
            }

            return startIndex;
        }

        public void SetValue(T value)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i].Equals(value))
                {
                    CurrentIndex = i;
                    break;
                }
            }
        }

        public override void OnMove()
        {
            valueTextBox.Text = CurrentValue.ToString();
        }
    }
}
