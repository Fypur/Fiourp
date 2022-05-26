using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class Switcher<T> : UIElement where T : Enum
    {
        public T CurrentValue { get; private set; }
        private int currentIndex; 
        private T[] Values;
        public Switcher(Vector2 position, int width, int height, bool centered, T startValue) : base(position, width, height, centered, new Sprite(Color.White))
        {
            CurrentValue = startValue;
            Values = (T[])Enum.GetValues(typeof(T));
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

        public void SetValue(T value)
        {
            CurrentValue = value;
            for(int i = 0; i < Values.Length; i++)
            {
                if(Values[i].Equals(value))
                {
                    currentIndex = i;
                    break;
                }
            }
        }

        public void GoLeft()
        {
            if (currentIndex <= 0)
            {
                NotPossible();
                return;
            }

            currentIndex--;
            CurrentValue = Values[currentIndex];
        }

        public void GoRight()
        {
            if (currentIndex >= Values.Length)
            {
                NotPossible();
                return;
            }

            currentIndex++;
            CurrentValue = Values[currentIndex];
        }

        public void NotPossible()
        {
            //TODO: Change this to shaking or smth
            Sprite.Color = Color.Black;
            AddComponent(new Timer(1, true, null, () => Sprite.Color = Color.White));
        }
    }
}
