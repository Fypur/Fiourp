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
        private const int maxLightSize = 512;
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

        /*public static void DrawLight(Vector2 position, Vector2 position2, Vector2 position3, Vector2 position4, Color color)
        {
            if (radius > maxLightSize / 2)
                throw new Exception("Light's Size is over the maximum size");

            if (lightNum + 1 > lights.Length)
                throw new Exception("Too much lights drawn at the same time");

            Vector2 renderTargetPos = new Vector2(maxLightSize * (lightNum % numLightsPerRow), maxLightSize * (lightNum / numLightsPerRow));
            Light l = new Light(position, renderTargetPos, radius, insideColor, outsideColor);


            lights[lightNum] = l;


            lightNum++;
        }*/

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

            public void Draw()
            {
                Drawing.DrawCircle(RenderTargetPosition + new Vector2(maxLightSize) / 2, Radius, 0.1f, InsideColor, OutsideColor);
            }
        }


        public static void FlushLights()
        {
            var lvl = Engine.CurrentMap.CurrentLevel;
            for (int i = 0; i < lightNum; i++)
            {
                Light l = lights[i];
                //Color c = new Color(Color.White, 255);
                l.Draw();

                Drawing.DrawCircle(lights[i].RenderTargetPosition + new Vector2(maxLightSize) / 2, lights[i].Radius, 0.1f, lights[i].InsideColor, lights[i].OutsideColor);
                //Drawing.DrawCircle(lights[i].RenderTargetPosition + new Vector2(maxLightSize) / 2, lights[i].Radius, 0.1f, c, c);

                for (int y = 0; y < lvl.ChunksEdge.GetLength(0); y++)
                    for(int x = 0; x < lvl.ChunksEdge.GetLength(1); x++)
                    {

                        if (!Collision.RectCircle(new Rectangle((int)lvl.Pos.X + x * lvl.ChunkSize * lvl.TileWidth, (int)lvl.Pos.Y + y * lvl.ChunkSize * lvl.TileHeight, lvl.ChunkSize * lvl.TileWidth, lvl.ChunkSize * lvl.TileHeight), lights[i].WorldPosition, lights[i].Radius))
                            continue;

                        foreach (int[] edge in lvl.ChunksEdge[y, x])
                        {

                            Vector2 center = lights[i].WorldPosition;
                            Vector2 pos1 = new Vector2(edge[0] * lvl.TileWidth, edge[1] * lvl.TileHeight) + lvl.Pos;
                            Vector2 pos2 = new Vector2(edge[2] * lvl.TileWidth, edge[3] * lvl.TileHeight) + lvl.Pos;

                            Vector2[] intersection = Collision.LineCircleIntersection(pos1, pos2, l.WorldPosition, l.Radius);

                            if (intersection.Length == 0 && (Vector2.DistanceSquared(pos1, center) > l.Radius * l.Radius && Vector2.DistanceSquared(pos2, center) > l.Radius * l.Radius))
                                continue;

                            Vector2 maxL = new Vector2(maxLightSize);

                            Vector2 pos3 = (pos1 - lights[i].WorldPosition).Normalized() * maxLightSize * 2f + maxL / 2;
                            Vector2 pos4 = (pos2 - lights[i].WorldPosition).Normalized() * maxLightSize * 2f + maxL / 2;


                            //TODO: LINEBOX INTERSECTION TO FIND POS3 AND POS4

                            /*if (!TestIntersect(ref pos3, Vector2.One, maxL.OnlyX() + Vector2.UnitY))
                                if (!TestIntersect(ref pos3, Vector2.One, maxL.OnlyY() + Vector2.UnitX))
                                    if (!TestIntersect(ref pos3, maxL.OnlyX() - Vector2.UnitX, maxL - Vector2.One))
                                        TestIntersect(ref pos3, maxL.OnlyY() - Vector2.UnitY, maxL - Vector2.One);

                            if (!TestIntersect(ref pos4, Vector2.One, maxL.OnlyX() + Vector2.UnitY))
                                if (!TestIntersect(ref pos4, Vector2.One, maxL.OnlyY() + Vector2.UnitX))
                                    if (!TestIntersect(ref pos4, maxL.OnlyX() - Vector2.UnitX, maxL - Vector2.One))
                                        TestIntersect(ref pos4, maxL.OnlyY() - Vector2.UnitY, maxL - Vector2.One);*/

                            if (!TestIntersect(ref pos3, Vector2.Zero, maxL.OnlyX()))
                                if (!TestIntersect(ref pos3, Vector2.Zero, maxL.OnlyY()))
                                    if (!TestIntersect(ref pos3, maxL.OnlyX(), maxL))
                                        TestIntersect(ref pos3, maxL.OnlyY(), maxL);

                            if (!TestIntersect(ref pos4, Vector2.Zero, maxL.OnlyX()))
                                if (!TestIntersect(ref pos4, Vector2.Zero, maxL.OnlyY()))
                                    if (!TestIntersect(ref pos4, maxL.OnlyX(), maxL))
                                        TestIntersect(ref pos4, maxL.OnlyY(), maxL);

                            bool TestIntersect(ref Vector2 pos, Vector2 box1, Vector2 box2)
                            {
                                var v = Collision.LineIntersection(maxL / 2, pos, box1, box2);
                                if (v is Vector2 u)
                                {
                                    pos = u;
                                    return true;
                                }
                                return false;
                            }

                            Vector2 mid = (pos3 + pos4) / 2;
                            Vector2 diff = VectorHelper.Abs(pos3 - pos4);

                            pos3 += lights[i].RenderTargetPosition;
                            pos4 += lights[i].RenderTargetPosition;


                            //Drawing.DrawQuad(pos3, pos4, l.RenderTargetPosition + maxL.OnlyY(), l.RenderTargetPosition + maxL, Color.Black);
                            if (mid.X != 0 && mid.Y != 0 && Vector2.DistanceSquared(mid, maxL / 2) < l.Radius * l.Radius) 
                            {
                                //Debug.LogUpdate(mid);
                                if (diff.X == maxLightSize)
                                {
                                    if(mid.Y < maxLightSize / 2)
                                        Drawing.DrawQuad(pos3, pos4, l.RenderTargetPosition, l.RenderTargetPosition + maxL.OnlyX(), Color.Transparent); //Top
                                    else
                                        Drawing.DrawQuad(pos3, pos4, l.RenderTargetPosition + maxL.OnlyY(), l.RenderTargetPosition + maxL, Color.Transparent); //Bottom
                                }

                                if(diff.Y == maxLightSize)
                                {
                                    //Debug.PointUpdate(Color.Orange, pos1, pos2);
                                    if (mid.X < maxLightSize / 2)
                                        Drawing.DrawQuad(pos3, pos4, l.RenderTargetPosition, l.RenderTargetPosition + maxL.OnlyY(), Color.Transparent); //Left
                                    else
                                        Drawing.DrawQuad(pos3, pos4, l.RenderTargetPosition + maxL.OnlyX(), l.RenderTargetPosition + maxL, Color.Transparent); //Right
                                }
                            }

                            //Debug.PointUpdate(Color.Orange, pos1, pos2, pos3 + center - new Vector2(maxLightSize) / 2 - l.RenderTargetPosition, pos4 + center - new Vector2(maxLightSize) / 2 - l.RenderTargetPosition);
                            //Debug.PointUpdate(Color.Orange, mid + center - new Vector2(maxLightSize) / 2);
                            //Debug.LogUpdate(Input.MousePos);
                            pos1 += lights[i].RenderTargetPosition + new Vector2(maxLightSize) / 2 - center;
                            pos2 += lights[i].RenderTargetPosition + new Vector2(maxLightSize) / 2 - center;



                            //Debug.PointUpdate(Color.Orange, pos1, pos2, pos3, pos4);
                            //Debug.PointUpdate(Color.Orange, pos1, pos2);
                            //Drawing.DrawLine(pos1, pos2, Color.Orange);
                            //Debug.PointUpdate(Color.Orange, pos1, pos2);

                            //pos1 -= center + new Vector2(maxLightSize) + lights[i].WorldPosition; ;
                            //pos1 = Vector2.One * 100;
                            //pos1 += new Vector2(maxLightSize) + lights[i].WorldPosition;

                            //Drawing.DrawQuad(pos1, pos1 + new Vector2(5, 0), pos1 + new Vector2(5, 5), pos1 + new Vector2(0, 5), Color.Blue);




                            Drawing.DrawQuad(pos1, pos2, pos4, pos3, Color.Transparent);
                        }
                    }
            }
        }

        public static void DrawAllLights()
        {
            for (int i = 0; i < lightNum; i++)
            {
                Light l = lights[i];

                Drawing.Draw(Engine.LightsRenderTarget, l.WorldPosition - new Vector2(maxLightSize) / 2, new Rectangle(l.RenderTargetPosition.ToPoint(), new Point(maxLightSize, maxLightSize)), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None);
            }

            lightNum = 0;
        }
    }
}
