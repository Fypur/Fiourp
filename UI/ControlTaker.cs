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
        public Color TextColor;
        public Color SelectedTextColor = new Color(255, 255, 120);

        public ControlTaker(Vector2 position, int width, int height, bool centered, Sprite sprite, Color textColor, string fontID, int fontSize, string fieldName, ControlList modified, Action onChange) : base(position, width, height, centered, sprite)
        {
            TextColor = textColor;

            FieldTextBox = (TextBox)AddChild(new TextBox(fieldName, fontID, Pos, width / 2, height, fontSize, TextColor, false, TextBox.Alignement.Left));
            valueTextBox = (TextBox)AddChild(new TextBox(modified.GetAllControlNames(), fontID, Pos + HalfSize.OnlyX(), width / 2, height, fontSize, TextColor, false, TextBox.Alignement.Right));

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

            if (Parent is SubMenu s && s.CanBack)
                s.CanBack = !recording;

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

                    if(Sprite != null)
                        Sprite.Color = Color.DarkRed;
                    else
                    {
                        FieldTextBox.Color = Color.Red;
                        valueTextBox.Color = Color.Red;
                    }
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

                if (Parent is SubMenu sub)
                    sub.CanBack = true;

                return;
            }

            var pressedControls = Input.GetPressedControls();

            //Waiting for pressed Button
            if (pressedControls.Count == 0)
                return;

            //Ending recording
            recording = false;
            if(Sprite != null)
                Sprite.Color = Color.White;
            else
            {
                FieldTextBox.Color = new Color(255, 255, 120);
                valueTextBox.Color = new Color(255, 255, 120);
            }

            if (Parent is SubMenu s)
                s.CanBack = true;

            if (Controls.Contains(pressedControls[0]))
                return;

            //More than 4 buttons => Clear the ControlList
            if (Controls.Count >= 4)
                Controls.Clear();


            //Add the control
            Controls.Add(pressedControls[0]);

            valueTextBox.SetText(Controls.GetAllControlNames());
        }

        public override void OnSelected()
        {
            base.OnSelected();

            FieldTextBox.Color = SelectedTextColor;
            valueTextBox.Color = SelectedTextColor;
        }

        public override void OnLeaveSelected()
        {
            base.OnLeaveSelected();

            FieldTextBox.Color = TextColor;
            valueTextBox.Color = TextColor;
        }
    }
}
