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
        public static Vector2 CurrentResolution;
        public static int DefaultUISizeMultiplier = 4;
        public static Vector2 DefaultScreenSize => RenderTargetSize * DefaultUISizeMultiplier;

        public static int CurrentScreenSizeMultiplier = DefaultUISizeMultiplier;
        private static Vector2 RenderTargetSize => Engine.RenderTarget.Bounds.Size.ToVector2();
        private static Vector2 resolutionBeforeFullScreen;

        public static void FullScreen()
        {
            GraphicsDeviceManager graphics = Engine.Graphics;
            
            if (!Engine.Graphics.IsFullScreen)
            {
                resolutionBeforeFullScreen = CurrentResolution;
                SetSize((int)(graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width / RenderTargetSize.X));
                graphics.ToggleFullScreen();
            }

            else
            {
                graphics.ToggleFullScreen();
                SetSize((int)(resolutionBeforeFullScreen.X / RenderTargetSize.X));
            }
        }

        public static void SetSize(int multiplier)
        {
            CurrentScreenSizeMultiplier = multiplier;

            int oldMult = (int)(CurrentResolution.X / RenderTargetSize.X);

            foreach (UIElement element in Engine.CurrentMap.Data.UIElements)
                SetUIStatsForSize(element, oldMult, multiplier);

            SetScreenSize(RenderTargetSize * multiplier);
            CurrentResolution = Engine.ScreenSize;
        }

        private static void SetUIStatsForSize(UIElement element, int oldMult, int newMult)
        {
            void SetStats(UIElement element, int oldMult, int newMult)
            {
                element.Pos = element.Pos / oldMult * newMult;
                element.PreviousExactPos = element.ExactPos;
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
