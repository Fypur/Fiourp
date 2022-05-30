using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class ControlTaker : UIElement
    {
        public TextBox FieldTextBox;
        private TextBox valueTextBox;
        
        public ControlList Modified;
        public Action OnChange;

        private bool recording;
        private bool clearActive => Selected && !recording;
        private float ratio = (float)8 / 10;

        public ControlTaker(Vector2 position, int width, int height, bool centered, string fieldName, ControlList modified, Action onChange) : base(position, width, height, centered, new Sprite(Color.White))
        {
            FieldTextBox = (TextBox)AddChild(new TextBox(fieldName, "LexendDeca", Pos, width / 2, height, Color.Black, 1, true));
            valueTextBox = (TextBox)AddChild(new TextBox(modified.GetAllControlNames(), "LexendDeca", Pos + HalfSize.OnlyX(), width / 2, height, Color.Black, 1, true));

            Modified = modified;
            OnChange = onChange;
        }

        public override void Update()
        {
            if(!((Input.UIAction1.IsDown() || Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))) && !recording)
                base.Update();

            if (Selected)
            {
                if (Input.ButtonClear.IsDown() && !recording)
                {
                    Modified.Clear();
                    valueTextBox.SetText(Modified.GetAllControlNames());
                }
                else if ((Input.UIAction1.IsDown() || Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter)) && !recording)
                {
                    recording = true;
                    Sprite.Color = Color.DarkRed;
                    //Return to not record the input that is pressed to enable recording
                    return;
                }

                if (recording)
                    Record();
            }
        }

        private void Record()
        {
            //Canceling recording
            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                recording = false;
                Sprite.Color = Color.White;
                return;
            }

            var pressedControls = Input.GetPressedControls();

            //Waiting for pressed Button
            if (pressedControls.Count == 0)
                return;

            //Ending recording
            recording = false;
            Sprite.Color = Color.White;

            if (Modified.Contains(pressedControls[0]))
                return;

            //More than 4 buttons => Clear the ControlList
            if (Modified.Count >= 4)
                Modified.Clear();

            //Add the control
            Modified.Add(pressedControls[0]);

            valueTextBox.SetText(Modified.GetAllControlNames());
        }
    }
}
