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
        private static Vector2 RenderTargetSize => Engine.RenderTarget.Bounds.Size.ToVector2();
        public static void FullScreen()
        {
            GraphicsDeviceManager graphics = Engine.Graphics;
            if (Engine.ScreenSize == CurrentResolution)
            {
                SetScreenSize(new Vector2(graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width, graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height));
                graphics.ToggleFullScreen();
            }

            else
            {
                graphics.ToggleFullScreen();
                SetScreenSize(CurrentResolution);
            }
        }

        public static void SetSize(int multiplier)
        {
            SetScreenSize(RenderTargetSize * multiplier);
            CurrentResolution = Engine.ScreenSize;
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
