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
            var lvl = Engine.CurrentMap.CurrentLevel;
            for (int i = 0; i < lightNum; i++)
            {
                Light l = lights[i];
                //Color c = new Color(Color.White, 100);
                Drawing.DrawCircle(lights[i].RenderTargetPosition + new Vector2(maxLightSize) / 2, lights[i].Radius, 0.1f, lights[i].InsideColor, lights[i].OutsideColor);

                /*if (i == 2)
                    Drawing.DrawCircle(lights[i].RenderTargetPosition + new Vector2(maxLightSize) / 2, lights[i].Radius, 0.1f, c, c);
                else
                    continue;*/
                //Drawing.DrawQuad(lights[i].RenderTargetPosition, c, lights[i].RenderTargetPosition + new Vector2(100, 0), c, lights[i].RenderTargetPosition + new Vector2(100, 100), c, lights[i].RenderTargetPosition + new Vector2(0, 100), c);
                //Drawing.DrawQuad(Input.MousePos, Color.White, Input.MousePos + new Vector2(100, 0), Color.White, Input.MousePos + new Vector2(100, 100), Color.White, Input.MousePos + new Vector2(0, 100), Color.White);

                //TODO: Draw Edge Quad for each light

                for (int y = 0; y < lvl.ChunksEdge.GetLength(0); y++)
                    for(int x = 0; x < lvl.ChunksEdge.GetLength(1); x++)
                    {
                        if (Collision.RectCircle(new Rectangle((int)lvl.Pos.X + x * lvl.ChunkSize * lvl.TileWidth, (int)lvl.Pos.Y + y * lvl.ChunkSize * lvl.TileHeight, lvl.ChunkSize * lvl.TileWidth, lvl.ChunkSize * lvl.TileHeight), lights[i].WorldPosition, lights[i].Radius))
                        {
                            foreach (int[] edge in lvl.ChunksEdge[y, x])
                            {

                                Vector2 center = lights[i].WorldPosition;
                                Vector2 pos1 = new Vector2(edge[0] * lvl.TileWidth, edge[1] * lvl.TileHeight) + lvl.Pos;
                                Vector2 pos2 = new Vector2(edge[2] * lvl.TileWidth, edge[3] * lvl.TileHeight) + lvl.Pos;

                                //Vector2[] intersection = Collision.LineCircleIntersection(pos1, pos2, l.WorldPosition, l.Radius);

                                /*if (intersection.Length <= 1)
                                {
                                    bool p1In = Vector2.DistanceSquared(pos1, center) < l.Radius * l.Radius;
                                    bool p2In = Vector2.DistanceSquared(pos2, center) < l.Radius * l.Radius;

                                    //Both aren't in the circle
                                    if(!p1In || !p2In)
                                    {
                                        if (intersection.Length == 0)
                                            continue;

                                        if (p1In)
                                            pos2 = intersection[0];
                                        else if (p2In)
                                            pos1 = intersection[0];
                                        else
                                            continue;
                                    }
                                }
                                else
                                {
                                    pos1 = intersection[0];
                                    pos2 = intersection[intersection.Length == 2 ? 1 : 0];
                                }*/



                                Vector2 pos3 = (pos1 - lights[i].WorldPosition).Normalized() * maxLightSize * 2;
                                Vector2 pos4 = (pos2 - lights[i].WorldPosition).Normalized() * maxLightSize * 2;


                                //TODO: LINEBOX INTERSECTION TO FIND POS3 AND POS4
                                /*for(int i = 0; i < 4; i++)
                                {
                                    pos3 = Collision.LineIntersection(Vector2.Zero, pos3, )
                                }*/





                                //Debug.PointUpdate(Color.Orange, pos1, pos2, pos3, pos4);
                                //Debug.PointUpdate(Color.Orange, pos1, pos2);
                                Debug.PointUpdate(Color.Orange, pos1, pos2, pos3 + center, pos4 + center);
                                //Drawing.DrawLine(pos1, pos2, Color.Orange);
                                //Debug.PointUpdate(Color.Orange, pos1, pos2);

                                //pos1 -= center + new Vector2(maxLightSize) + lights[i].WorldPosition; ;
                                //pos1 = Vector2.One * 100;
                                //pos1 += new Vector2(maxLightSize) + lights[i].WorldPosition;

                                //Drawing.DrawQuad(pos1, pos1 + new Vector2(5, 0), pos1 + new Vector2(5, 5), pos1 + new Vector2(0, 5), Color.Blue);

                                pos1 += lights[i].RenderTargetPosition + new Vector2(maxLightSize) / 2 - center;
                                pos2 += lights[i].RenderTargetPosition + new Vector2(maxLightSize) / 2 - center;
                                pos3 += lights[i].RenderTargetPosition + new Vector2(maxLightSize) / 2;
                                pos4 += lights[i].RenderTargetPosition + new Vector2(maxLightSize) / 2;

                                /*if (pos3.X < 0) pos3.X = 0; if (pos3.X > maxLightSize) pos3.X = maxLightSize;
                                if (pos4.X < 0) pos4.X = 0; if (pos4.X > maxLightSize) pos4.X = maxLightSize;
                                if (pos3.Y < 0) pos3.Y = 0; if (pos3.Y > maxLightSize) pos3.Y = maxLightSize;
                                if (pos4.Y < 0) pos4.Y = 0; if (pos4.Y > maxLightSize) pos4.Y = maxLightSize;*/

                                Drawing.DrawQuad(pos1, pos2, pos4, pos3, Color.Black);
                            }
                        }
                    }
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
