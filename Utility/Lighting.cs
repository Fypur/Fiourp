using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public static class Lighting
    {
        public const int LightsTargetSize = 2048;
        private const int maxLightSize = 256;
        private const int numLightsPerRow = LightsTargetSize / maxLightSize;

        private static RenderTarget2D lightsTarget => Engine.LightsRenderTarget;
        private static RenderTarget2D mainTarget => Engine.RenderTarget;

        private static Light[] lights = new Light[LightsTargetSize / maxLightSize * LightsTargetSize / maxLightSize];
        private static int lightNum;

        public static void DrawLight(Vector2 position, float radius, Color insideColor, Color outsideColor)
        {
            if (radius > maxLightSize / 2)
                throw new Exception("Light's Size is over the maximum size");

            if (lightNum + 1 > lights.Length)
                throw new Exception("Too much lights drawn at the same time");

            Vector2 renderTargetPos = new Vector2(maxLightSize * (lightNum % numLightsPerRow), maxLightSize * (lightNum / numLightsPerRow));
            Light l = new Light(position, renderTargetPos, radius, insideColor, outsideColor);


            lights[lightNum] = l;


            lightNum++;
        }

        public class Light
        {
            public Vector2 WorldPosition;
            public Vector2 RenderTargetPosition;
            public float Radius;
            public Color InsideColor;
            public Color OutsideColor;

            public Light(Vector2 worldPosition, Vector2 renderTargetPosition, float radius, Color insideColor, Color outsideColor)
            {
                WorldPosition = worldPosition;
                RenderTargetPosition = renderTargetPosition;
                Radius = radius;
                InsideColor = insideColor;
                OutsideColor = outsideColor;
            }
        }


        public static void FlushLights()
        {
            //Drawing.DrawQuad(new Vector2(0, 0), Color.White, new Vector2(100, 0), Color.White, new Vector2(100, 100), Color.White, new Vector2(0, 100), Color.White);

            //Drawing.DrawCircle(new Vector2(0, 0) + new Vector2(50) / 2, 100, 0.1f, Color.White, Color.Transparent);

            for (int i = 0; i < lightNum; i++)
            {
                Color c = new Color(Color.White, 200);
                Drawing.DrawCircle(lights[i].RenderTargetPosition + new Vector2(maxLightSize) / 2, lights[i].Radius, 0.1f, lights[i].InsideColor, lights[i].OutsideColor);
                //Drawing.DrawCircle(lights[i].RenderTargetPosition + new Vector2(maxLightSize) / 2, lights[i].Radius, 0.1f, lights[i].InsideColor, lights[i].OutsideColor);



                //Drawing.DrawQuad(lights[i].RenderTargetPosition, c, lights[i].RenderTargetPosition + new Vector2(100, 0), c, lights[i].RenderTargetPosition + new Vector2(100, 100), c, lights[i].RenderTargetPosition + new Vector2(0, 100), c);


                //Drawing.DrawQuad(Input.MousePos, Color.White, Input.MousePos + new Vector2(100, 0), Color.White, Input.MousePos + new Vector2(100, 100), Color.White, Input.MousePos + new Vector2(0, 100), Color.White);

                //TODO: Draw Edge Quad for each light
            }
        }

        public static void DrawAllLights()
        {
            //Drawing.Draw(Engine.LightsRenderTarget, Vector2.Zero, new Rectangle(new Point(0, 0), new Point(maxLightSize, maxLightSize)), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None);

            for (int i = 0; i < lightNum; i++)
            {
                Light l = lights[i];

                Drawing.Draw(Engine.LightsRenderTarget, l.WorldPosition - new Vector2(maxLightSize) / 2, new Rectangle(l.RenderTargetPosition.ToPoint(), new Point(maxLightSize, maxLightSize)), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None);
                //Drawing.Draw(Engine.LightsRenderTarget, l.WorldPosition - new Vector2(78) / 2, new Rectangle(l.RenderTargetPosition.ToPoint(), new Point(maxLightSize, maxLightSize)), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None);



                //Drawing.Draw(Engine.LightsRenderTarget, l.WorldPosition - new Vector2(maxLightSize) / 2, new Rectangle(l.RenderTargetPosition.ToPoint(), new Point(maxLightSize, maxLightSize)), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None);

                //Drawing.Draw(Engine.LightsRenderTarget, l.WorldPosition - new Vector2(maxLightSize) / 2, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None);

                //Drawing.Draw(Engine.LightsRenderTarget, Input.MousePos - new Vector2(maxLightSize) / 2, new Rectangle(l.RenderTargetPosition.ToPoint(), new Point(maxLightSize, maxLightSize)), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None);
            }

            lightNum = 0;
        }
    }
}
