using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public static class Options
    {
        private static int lowestResolutionX = 320;
        public static int DefaultUISizeMultiplier = 4;
        public static Vector2 DefaultScreenSize => new Vector2(lowestResolutionX, lowestResolutionX / 16 * 9) * DefaultUISizeMultiplier;

        public static int CurrentScreenSizeMultiplier { get; private set; }
        private static int resolutionMultiplierBeforeFullScreen;

        static Options()
        {
            CurrentScreenSizeMultiplier = DefaultUISizeMultiplier;
        }

        public static void FullScreen()
        {
            GraphicsDeviceManager graphics = Engine.Graphics;
            
            if (!Engine.Graphics.IsFullScreen)
            {
                resolutionMultiplierBeforeFullScreen = CurrentScreenSizeMultiplier;
                SetSize((int)(graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width / lowestResolutionX));
                graphics.ToggleFullScreen();
            }

            else
            {
                graphics.ToggleFullScreen();
                SetSize(resolutionMultiplierBeforeFullScreen);
            }
        }

        public static void SetSize(int multiplier)
        {
            int oldMult = CurrentScreenSizeMultiplier;
            CurrentScreenSizeMultiplier = multiplier;

            foreach (UIElement element in Engine.CurrentMap.Data.UIElements)
                SetUIStatsForSize(element, oldMult, multiplier);

            SetScreenSize(new Vector2(lowestResolutionX, lowestResolutionX / 16 * 9) * multiplier);
        }

        private static void SetUIStatsForSize(UIElement element, int oldMult, int newMult)
        {
            void SetStats(UIElement element, int oldMult, int newMult)
            {
                element.PreviousPos += element.Pos / oldMult * newMult - element.Pos;
                element.PreviousExactPos += element.Pos - element.Pos / oldMult * newMult;

                element.Pos = element.Pos / oldMult * newMult;
                element.Size = element.Size / oldMult * newMult;
                element.OnSizeChange();
            }

            SetStats(element, oldMult, newMult);
            foreach (UIElement child in element.Children)
                SetUIStatsForSize(child, oldMult, newMult);
        }

        private static void SetScreenSize(Vector2 screenSize)
        {
            GraphicsDeviceManager graphics = Engine.Graphics;
            Engine.ScreenSize = screenSize;
            graphics.PreferredBackBufferWidth = (int)Engine.ScreenSize.X;
            graphics.PreferredBackBufferHeight = (int)Engine.ScreenSize.Y;
            graphics.ApplyChanges();
        }
    }
}
