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

        public static void Initialize(GraphicsDeviceManager graphicsDevice, ContentManager content, int windowsWidth, int windowHeight, RenderTarget2D renderTarget, string XMLPath)
        {
            Graphics = graphicsDevice;
            Content = content;
            graphicsDevice.PreferredBackBufferWidth = windowsWidth;
            graphicsDevice.PreferredBackBufferHeight = windowHeight;
            graphicsDevice.ApplyChanges();

            ScreenSize = new Vector2(graphicsDevice.PreferredBackBufferWidth, graphicsDevice.PreferredBackBufferHeight);

            RenderTarget = renderTarget;

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
            CurrentMap.Update();
        }

        public static void LateUpdate() 
        {
            Input.UpdateOldState();
            Audio.Update();
        }
    }
}
