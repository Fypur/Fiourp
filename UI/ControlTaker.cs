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
        public ControlList Controls;
        public ControlList KbMouseControls = new();
        public ControlList GamepadControls = new();

        public Action OnChange;

        private bool recording;

        public ControlTaker(Vector2 position, int width, int height, bool centered, string fieldName, ControlList modified, Action onChange) : base(position, width, height, centered, new Sprite(Color.White))
        {
            FieldTextBox = (TextBox)AddChild(new TextBox(fieldName, "Recursive", Pos, width / 2, height, 0.5f, Color.Black, true));
            valueTextBox = (TextBox)AddChild(new TextBox(modified.GetAllControlNames(), "Recursive", Pos + HalfSize.OnlyX(), width / 2, height, 0.3f, Color.Black, true));

            Modified = modified;
            foreach(Control control in modified)
            {
                if(control.Key != null || control.MouseButton != null)
                    KbMouseControls.Add(control);
                else
                    GamepadControls.Add(control);
            }

            OnChange = onChange;
        }

        public override void Update()
        {
            if(!((Input.UIAction1.IsDown() || Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))) && !recording)
                base.Update();

            List<Control> mod = new List<Control>(KbMouseControls.Controls);
            mod.AddRange(GamepadControls);
            Modified.Controls = mod;

            if (Input.GamePadConnected)
                Controls = GamepadControls;
            else
                Controls = KbMouseControls;

            valueTextBox.SetText(Controls.GetAllControlNames());

            if (Selected)
            {
                if (Input.ButtonClear.IsDown() && !recording)
                {
                    Controls.Clear();
                    valueTextBox.SetText(Controls.GetAllControlNames());
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

            if (Controls.Contains(pressedControls[0]))
                return;

            //More than 4 buttons => Clear the ControlList
            if (Controls.Count >= 4)
                Controls.Clear();


            //Add the control
            Controls.Add(pressedControls[0]);

            valueTextBox.SetText(Controls.GetAllControlNames());
        }
    }
}
