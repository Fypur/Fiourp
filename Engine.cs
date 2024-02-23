using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public static class Engine
    {
        public static GraphicsDeviceManager Graphics;
        public static ContentManager Content;

        public static Vector2 ScreenSize;
        public static float Deltatime;

        public static Camera Cam;
        public static Entity Player;
        public static Map CurrentMap;
        public static RenderTarget2D RenderTarget;
        public static RenderTarget2D PrimitivesRenderTarget;
        public static RenderTarget2D LightsRenderTarget;

        public static void Initialize(GraphicsDeviceManager graphicsDevice, ContentManager content, int windowsWidth, int windowHeight, RenderTarget2D renderTarget, string XMLPath)
        {
            Graphics = graphicsDevice;
            Content = content;
            graphicsDevice.PreferredBackBufferWidth = windowsWidth;
            graphicsDevice.PreferredBackBufferHeight = windowHeight;
            graphicsDevice.ApplyChanges();

            ScreenSize = new Vector2(graphicsDevice.PreferredBackBufferWidth, graphicsDevice.PreferredBackBufferHeight);

            if (renderTarget == null) renderTarget = new(graphicsDevice.GraphicsDevice, windowsWidth, windowHeight);

            RenderTarget = renderTarget;
            PrimitivesRenderTarget = new RenderTarget2D(renderTarget.GraphicsDevice, renderTarget.Width, renderTarget.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            LightsRenderTarget = new RenderTarget2D(renderTarget.GraphicsDevice, Lighting.LightsTargetSize, Lighting.LightsTargetSize, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            //graphicsDevice.GraphicsDevice.SetRenderTargets(RenderTarget, PrimitivesRenderTarget);

            if (XMLPath != "")
            {
                DataManager.Initialize(XMLPath);
                Sprite.LoadAnimationXML(XMLPath);
            }
            else
                DataManager.Initialize();
            
            Audio.Initialize();
        }

        public static void Update(GameTime gameTime)
        {
            Input.UpdateState();
            Deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            CurrentMap?.Update();
        }

        public static void LateUpdate() 
        {
            Input.UpdateOldState();
            Audio.Update();
        }
    }
}
