using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class WritingTextBox : TextBox
    {
        public WritingTextBox(string text, string fontID, Vector2 position, int width, int height, float fontSize, Color color, bool centeredUI, Alignement alignement, float timePerCharacter)
            : base("", fontID, position, width, height, fontSize, color, centeredUI, alignement)
        {
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

        public void Reset()
        {
            StopAllCoroutines();
            Text = "";
        }

        private IEnumerator TextDraw(string text, float timePerCharacter)
        {
            foreach (char c in text)
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
    }
}
