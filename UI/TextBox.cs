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
        public string Text { get; protected set; }
        public string FontID;
        protected float textScale;
        public bool CenteredText;
        public float TextScale { get => textScale / Options.DefaultUISizeMultiplier * Options.CurrentScreenSizeMultiplier; set => textScale = value; }

        public Color Color = Color.White;

        public SpriteFont Font => DataManager.Fonts[FontID]["Normal"];

        public enum Style { Normal, Bold, Italic }

        public TextBox(string text, string fontID, Vector2 position, int width, int height, float fontSize, Color color, bool centeredUI = false, bool centeredText = false)
            : base(position, width, height, centeredUI, null)
        {
            FontID = fontID;
            textScale = fontSize;
            Text = GenerateText(text);
            CustomCenter = true;
            Centered = centeredUI;

            if (centeredUI && !centeredText)
                CenteredText = true;
            else
                CenteredText = centeredText;

            Color = color;
        }

        public override void Awake()
        {
            base.Awake();
            Collider.DebugColor = Color.Cyan;
            selectableField = false;
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

                if (lineSize > Width && newText != "")
                {
                    newText += "\n";
                    lineSize = wordSize;
                }

                newText += word + " ";
                lineSize += spaceSize;
            }

            return newText.TrimEnd();
        }

        public override void Render()
        {
            //(Font, Text, Pos, Color, rotation, origin, scale, effects, layerDepth);
            base.Render();

            if (Text != null)
            {
                if (!CenteredText)
                    Drawing.DrawString(Text, Pos, Color, Font, TextScale);
                else
                {
                    Vector2 textSize = Font.MeasureString(Text);
                    Drawing.DrawString(Text, Pos + HalfSize, Color, Font, textSize / 2, TextScale, 0);
                }        
            }
            if (Debug.DebugMode)
                Drawing.DrawEdge(new Rectangle(Pos.ToPoint(), Size.ToPoint()), 1, Color.Blue);
        }

        public override string ToString()
            => $"Pos: {Pos}, Font: {FontID}, Scale: {TextScale}, Text: {Text}";
    }
}
