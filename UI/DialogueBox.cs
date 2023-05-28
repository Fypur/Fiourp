using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class DialogueBox : UIElement
    {
        public WritingTextBox TextBox;
        public string[] Dialogue;

        private int currentTextIndex = 0;
        private string currentGeneratedString;
        private ControlList skipDialogControls;
        
        public DialogueBox(string[] dialogue, ControlList skipDialogueControls)
            : base(new Vector2(140, 20), 1000, 300, new Sprite(Color.White, new Rectangle(140, 20, 1000, 300), 0.9f))
        {
            Dialogue = dialogue;
            this.skipDialogControls = skipDialogueControls;
            TextBox = new WritingTextBox(dialogue[0], "Pixel", Pos + new Vector2(20, 20), Width - 30, Height - 20, 3, Color.White, false, Fiourp.TextBox.Alignement.TopLeft, 0.01f);
            currentGeneratedString = TextBox.GenerateText(dialogue[0]);

            AddChild(TextBox);
            AddComponent(new Sprite(Color.Black, new Rectangle(Pos.ToPoint() + new Point(10, 10), Size.ToPoint() - new Point(20, 20)), 0.8f));
        }

        public override void Update()
        {
            base.Update();
            if(skipDialogControls.IsDown())
            {
                if (TextBox.Text == currentGeneratedString)
                {
                    if(currentTextIndex + 1 >= Dialogue.Length)
                    {
                        Engine.CurrentMap.Destroy(this);
                        return;
                    }

                    currentTextIndex++;
                    currentGeneratedString = TextBox.GenerateText(Dialogue[currentTextIndex]);
                    TextBox.Reset();
                    TextBox.ProgressiveDraw(currentGeneratedString, 0.01f, true);
                }
                else
                {
                    TextBox.StopAllCoroutines();
                    TextBox.SetText(currentGeneratedString);
                }
            }
        }
    }
}
