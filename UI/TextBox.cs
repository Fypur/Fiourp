using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class TextBox : UIElement
    {
        public string Text;
        public string FontID;
        public float TextScale;

        public Color Color = Color.White;

        public SpriteFont Font => DataManager.Fonts[FontID]["Normal"];

        public enum Style { Normal, Bold, Italic }

        public TextBox(string text, string fontID, Vector2 position, int width, int height, Color color, float fontSize = 3, bool centered = false)
            : base(position, width, height, false, null)
        {
            FontID = fontID;
            TextScale = fontSize;
            Text = GenerateText(text);
            CustomCenter = true;
            Centered = centered;
            Color = color;
            Collider.DebugColor = Color.Cyan;
        }

        public TextBox(string text, string fontID, float timePerCharacter, Vector2 position, int width, int height, float fontSize = 3)
            : base(position, width, height, null)
        {
            CustomCenter = true;
            FontID = fontID;
            TextScale = fontSize;
            Collider.DebugColor = Color.Cyan;
            ProgressiveDraw(GenerateText(text), timePerCharacter);
        }

        public void ProgressiveDraw(string text, float timePerCharacter, bool formattedText = false)
            => AddComponent(new Coroutine(TextDraw(formattedText ? text : GenerateText(text), timePerCharacter)));

        public void ProgressiveRemove(float timePerCharacter)
            => AddComponent(new Coroutine(TextRemove(timePerCharacter)));

        public void StopAllCoroutines()
        {
            foreach (Coroutine c in GetComponents<Coroutine>())
                RemoveComponent(c);
        }

        private IEnumerator TextDraw(string text, float timePerCharacter)
        {
            foreach(char c in text)
            {
                Text += c;
                yield return new Coroutine.WaitForSeconds(timePerCharacter);
            }
        }

        private IEnumerator TextRemove(float timePerCharacter)
        {
            for (int i = Text.Length - 1; i >= 0; i--)
            {
                Text = Text.Remove(i);
                yield return new Coroutine.WaitForSeconds(timePerCharacter);
            }
        }

        public void SetText(string text)
            => Text = GenerateText(text);

        public void ClearText()
            => Text = "";

        public string GenerateText(string text)
        {
            string[] words = text.Split(" ");
            string newText = "";
            float lineSize = 0;
            float spaceSize = Font.MeasureString(" ").X * TextScale;
            foreach (string word in words)
            {
                float wordSize = Font.MeasureString(word).X * TextScale;
                lineSize += wordSize;

                if (word.Contains('\n'))
                    lineSize = wordSize;

                if (lineSize > Width)
                {
                    newText += "\n";
                    lineSize = wordSize;
                }

                newText += word + " ";
                lineSize += spaceSize;
            }

            return newText.TrimEnd();
        }

        public void Reset()
        {
            StopAllCoroutines();
            Text = "";
        }

        public override void Render()
        {
            //(Font, Text, Pos, Color, rotation, origin, scale, effects, layerDepth);
            base.Render();

            if (Text != null)
            {
                if (!Centered)
                    Drawing.DrawString(Text, Pos, Color, Font, TextScale);
                else
                {
                    Vector2 textSize = Font.MeasureString(Text);

                    Drawing.DrawString(Text, Pos + HalfSize,
                        Color,
                        DataManager.Fonts[FontID]["Normal"],
                        textSize / 2,
                        TextScale, 0);
                }



                    /*Drawing.DrawString(Text, Pos, Color, DataManager.Fonts[FontID]["Normal"], Input.MousePosNoRenderTarget - Pos, TextScale, 0);*/
                    //-HalfSize + measurestring / 2
                    /* Pos= new Vector2(Size.X + ((Size.Width - Font.MeasureString(Text).X) / 2), Size.Y + ((Size.Height - Font.MeasureString(Text).Y)) / 2)
                     Origin= new Vector2((Font.MeasureString(Text).X / 2 * scale - Font.MeasureString(Text).X / 2), (Font.MeasureString(Text).Y / 2 * scale - Font.MeasureString(Text).Y / 2)), scale);*/
                
                    
            }
            if (Debug.DebugMode)
                Drawing.DrawEdge(new Rectangle(Pos.ToPoint(), Size.ToPoint()), 1, Color.Blue);
        }

        public override string ToString()
            => $"Pos: {Pos}, Font: {FontID}, Scale: {TextScale}, Text: {Text}";
    }
}
