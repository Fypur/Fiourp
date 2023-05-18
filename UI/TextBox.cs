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
        public Alignement TextAlignement;
        public float TextScale { get => textScale / Options.DefaultUISizeMultiplier * Options.CurrentScreenSizeMultiplier; set => textScale = value; }

        public Color Color = Color.White;

        public SpriteFont Font => DataManager.Fonts[FontID]["Normal"];

        public enum Style { Normal, Bold, Italic }
        public enum Alignement { TopLeft, TopCenter, TopRight, Left, Center, Right, BottomLeft, BottomCenter, BottomRight }

        public TextBox(string text, string fontID, Vector2 position, int width, int height, float fontSize, Color color, bool centeredUI = false, Alignement alignement = Alignement.Center)
            : base(position, width, height, centeredUI, null)
        {
            FontID = fontID;
            textScale = fontSize;
            Text = GenerateText(text);
            CustomCenter = true;
            Centered = centeredUI;

            TextAlignement = alignement;

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
            float spaceSize = Font.MeasureString(" ").X * textScale;
            foreach (string word in words)
            {
                float wordSize = Font.MeasureString(word).X * textScale;
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
                Vector2 textSize = Font.MeasureString(Text);

                switch (TextAlignement)
                {
                    case Alignement.TopLeft:
                        Drawing.DrawString(Text, Pos, Color, Font, TextScale);
                        break;
                    case Alignement.TopCenter:
                        Drawing.DrawString(Text, Pos + HalfSize.OnlyX(), Color, Font, textSize.OnlyX() / 2, TextScale, 0);
                        break;
                    case Alignement.TopRight:
                        Drawing.DrawString(Text, Pos + Size.OnlyX() - textSize.OnlyX(), Color, Font, TextScale);
                        break;
                    case Alignement.Left:
                        Drawing.DrawString(Text, Pos + HalfSize.OnlyY() - (textSize / 2).OnlyY(), Color, Font, TextScale);
                        break;
                    case Alignement.Center:
                        Drawing.DrawString(Text, Pos + HalfSize, Color, Font, textSize / 2, TextScale, 0);
                        break;
                    case Alignement.Right:
                        Drawing.DrawString(Text, Pos + HalfSize.OnlyY() - (textSize / 2).OnlyY() + Size.OnlyX() - textSize.OnlyX(), Color, Font, TextScale);
                        break;
                    case Alignement.BottomLeft:
                        Drawing.DrawString(Text, Pos + Size.OnlyY() - textSize.OnlyY(), Color, Font, TextScale);
                        break;
                    case Alignement.BottomCenter:
                        Drawing.DrawString(Text, Pos + HalfSize.OnlyX() + Size.OnlyY() - textSize.OnlyX() / 2 - textSize.OnlyY(), Color, Font, TextScale);
                        break; 
                    case Alignement.BottomRight:
                        Drawing.DrawString(Text, Pos + Size - textSize, Color, Font, TextScale);
                        break;
                }
            }
            if (Debug.DebugMode)
                Drawing.DrawEdge(new Rectangle(Pos.ToPoint(), Size.ToPoint()), 1, Color.Blue);
        }

        public override string ToString()
            => $"Pos: {Pos}, Font: {FontID}, Scale: {TextScale}, Text: {Text}";
    }
}
