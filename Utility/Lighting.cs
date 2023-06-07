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
        public const int LightsTargetSize = 4096;
        public const int MaxLightSize = 512;
        private const int numLightsPerRow = LightsTargetSize / MaxLightSize;

        private static RenderTarget2D lightsTarget => Engine.LightsRenderTarget;
        private static RenderTarget2D mainTarget => Engine.RenderTarget;

        private static Light[] lights = new Light[LightsTargetSize / MaxLightSize * LightsTargetSize / MaxLightSize];
        private static int lightNum;

        public static void DrawLight(Light light)
        {
            if (light.OverSize())
                throw new Exception("Light's Size is over the maximum size");

            if (lightNum + 1 > lights.Length)
                throw new Exception("Too much lights drawn at the same time");

            light.RenderTargetPosition = new Vector2(MaxLightSize * (lightNum % numLightsPerRow), MaxLightSize * (lightNum / numLightsPerRow));

            lights[lightNum] = light;
            lightNum++;
        }


        public static void FlushLights()
        {
            var lvl = Engine.CurrentMap.CurrentLevel;
            for (int i = 0; i < lightNum; i++)
            {
                Light l = lights[i];
               

                l.DrawRenderTarget();

                if (!l.CollideWithWalls)
                    continue;

                for (int y = 0; y < lvl.ChunksEdge.GetLength(0); y++)
                    for(int x = 0; x < lvl.ChunksEdge.GetLength(1); x++)
                    {

                        if (!Collision.RectCircle(
                            new Rectangle((int)lvl.Pos.X + x * lvl.ChunkSize * lvl.TileWidth, (int)lvl.Pos.Y + y * lvl.ChunkSize * lvl.TileHeight, lvl.ChunkSize * lvl.TileWidth, lvl.ChunkSize * lvl.TileHeight), 
                            lights[i].WorldPosition, lights[i].Size))
                            continue;

                        foreach (int[] edge in lvl.ChunksEdge[y, x])
                        {
                            Vector2 lightWorldPos = lights[i].WorldPosition;
                            Vector2 edgePos1 = new Vector2(edge[0] * lvl.TileWidth, edge[1] * lvl.TileHeight) + lvl.Pos;
                            Vector2 edgePos2 = new Vector2(edge[2] * lvl.TileWidth, edge[3] * lvl.TileHeight) + lvl.Pos;
                            Vector2 maxL = new Vector2(MaxLightSize);

                            Vector2[] intersection = Collision.LineCircleIntersection(edgePos1, edgePos2, l.WorldPosition, l.Size);

                            if (intersection.Length == 0 && (Vector2.DistanceSquared(edgePos1, lightWorldPos) > l.Size * l.Size && Vector2.DistanceSquared(edgePos2, lightWorldPos) > l.Size * l.Size))
                                continue;


                            edgePos1 += maxL / 2 - lightWorldPos;
                            edgePos2 += maxL / 2 - lightWorldPos;

                            if (edgePos1.X < 0)
                                edgePos1.X = 0;
                            if (edgePos1.Y < 0)
                                edgePos1.Y = 0;
                            if (edgePos1.X > MaxLightSize)
                                edgePos1.X = MaxLightSize;
                            if (edgePos1.Y > MaxLightSize)
                                edgePos1.Y = MaxLightSize;

                            if (edgePos2.X < 0)
                                edgePos2.X = 0;
                            if (edgePos2.Y < 0)
                                edgePos2.Y = 0;
                            if (edgePos2.X > MaxLightSize)
                                edgePos2.X = MaxLightSize;
                            if (edgePos2.Y > MaxLightSize)
                                edgePos2.Y = MaxLightSize;

                            edgePos1 -= maxL / 2 - lightWorldPos;
                            edgePos2 -= maxL / 2 - lightWorldPos;




                            Vector2 projectedEdgePos1 = (edgePos1 - lightWorldPos).Normalized() * MaxLightSize * 2f + maxL / 2;
                            Vector2 projectedEdgePos2 = (edgePos2 - lightWorldPos).Normalized() * MaxLightSize * 2f + maxL / 2;


                            //TODO: LINEBOX INTERSECTION TO FIND POS3 AND POS4

                            if (!TestIntersect(ref projectedEdgePos1, Vector2.Zero, maxL.OnlyX()))
                                if (!TestIntersect(ref projectedEdgePos1, Vector2.Zero, maxL.OnlyY()))
                                    if (!TestIntersect(ref projectedEdgePos1, maxL.OnlyX(), maxL))
                                        TestIntersect(ref projectedEdgePos1, maxL.OnlyY(), maxL);

                            if (!TestIntersect(ref projectedEdgePos2, Vector2.Zero, maxL.OnlyX()))
                                if (!TestIntersect(ref projectedEdgePos2, Vector2.Zero, maxL.OnlyY()))
                                    if (!TestIntersect(ref projectedEdgePos2, maxL.OnlyX(), maxL))
                                        TestIntersect(ref projectedEdgePos2, maxL.OnlyY(), maxL);

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

                            Vector2 midProjected = (projectedEdgePos1 + projectedEdgePos2) / 2;
                            Vector2 diff = VectorHelper.Abs(projectedEdgePos1 - projectedEdgePos2);

                            projectedEdgePos1 += lights[i].RenderTargetPosition;
                            projectedEdgePos2 += lights[i].RenderTargetPosition;

                            //Debug.PointUpdate(Color.Orange, edgePos1 - lightWorldPos + lights[i].RenderTargetPosition + maxL / 2, edgePos2 - lightWorldPos + lights[i].RenderTargetPosition + maxL / 2);
                            //Debug.PointUpdate(Color.Yellow, projectedEdgePos1, projectedEdgePos2);

                            //Drawing.DrawQuad(pos3, pos4, l.RenderTargetPosition + maxL.OnlyY(), l.RenderTargetPosition + maxL, Color.Transparent);
                            if (midProjected.X != 0 && midProjected.Y != 0 && Vector2.DistanceSquared(midProjected, maxL / 2) < l.Size * l.Size) 
                            {
                                
                                //Debug.LogUpdate(mid);
                                if (diff.X == MaxLightSize)
                                {
                                    if (midProjected.Y < MaxLightSize / 2)
                                    Drawing.DrawQuad(projectedEdgePos1, projectedEdgePos2, l.RenderTargetPosition + maxL.OnlyX(), l.RenderTargetPosition, Color.Transparent); //Top
                                    else
                                        Drawing.DrawQuad(projectedEdgePos1, projectedEdgePos2, l.RenderTargetPosition + maxL.OnlyY(), l.RenderTargetPosition + maxL, Color.Transparent); //Bottom
                                }

                                if(diff.Y == MaxLightSize)
                                {
                                    //Debug.PointUpdate(Color.Orange, pos1, pos2);
                                    if (midProjected.X < MaxLightSize / 2)
                                        Drawing.DrawQuad(projectedEdgePos1, projectedEdgePos2, l.RenderTargetPosition, l.RenderTargetPosition + maxL.OnlyY(), Color.Transparent); //Left
                                    else
                                        Drawing.DrawQuad(projectedEdgePos1, projectedEdgePos2, l.RenderTargetPosition + maxL.OnlyX(), l.RenderTargetPosition + maxL, Color.Transparent); //Right
                                }
                            }




                            //Debug.PointUpdate(Color.Orange, edgePos1, edgePos2);
                            //Debug.PointUpdate(Color.Orange, projectedEdgePos1 + l.WorldPosition - l.RenderTargetPosition, projectedEdgePos2 + l.WorldPosition - l.RenderTargetPosition);

                            //Debug.PointUpdate(Color.Orange, pos1, pos2, pos3 + center - new Vector2(maxLightSize) / 2 - l.RenderTargetPosition, pos4 + center - new Vector2(maxLightSize) / 2 - l.RenderTargetPosition);
                            //Debug.PointUpdate(Color.Orange, mid + center - new Vector2(maxLightSize) / 2);
                            //Debug.LogUpdate(Input.MousePos);


                            edgePos1 += maxL / 2 - lightWorldPos;
                            edgePos2 += maxL / 2 - lightWorldPos;

                            if (edgePos1.X < 0)
                                edgePos1.X = 0;
                            if (edgePos1.Y < 0)
                                edgePos1.Y = 0;
                            if (edgePos1.X > MaxLightSize)
                                edgePos1.X = MaxLightSize;
                            if (edgePos1.Y > MaxLightSize)
                                edgePos1.Y = MaxLightSize;

                            if (edgePos2.X < 0)
                                edgePos2.X = 0;
                            if (edgePos2.Y < 0)
                                edgePos2.Y = 0;
                            if (edgePos2.X > MaxLightSize)
                                edgePos2.X = MaxLightSize;
                            if (edgePos2.Y > MaxLightSize)
                                edgePos2.Y = MaxLightSize;

                            edgePos1 += lights[i].RenderTargetPosition;
                            edgePos2 += lights[i].RenderTargetPosition;



                            //Debug.PointUpdate(Color.Orange, pos1, pos2, pos3, pos4);

                            //Drawing.DrawLine(pos1, pos2, Color.Orange);dq
                            //Debug.PointUpdate(Color.Orange, pos1, pos2);

                            //pos1 -= center + new Vector2(maxLightSize) + lights[i].WorldPosition; ;
                            //pos1 = Vector2.One * 100;
                            //pos1 += new Vector2(maxLightSize) + lights[i].WorldPosition;

                            //Drawing.DrawQuad(edgePos1, edgePos1 + new Vector2(5, 0), edgePos1 + new Vector2(5, 5), edgePos1 + new Vector2(0, 5), Color.Blue);



                            //Drawing.DrawQuad(edgePos1, edgePos2, projectedEdgePos2, projectedEdgePos1, Color.Transparent);
                            Drawing.DrawQuad(projectedEdgePos2, projectedEdgePos1, edgePos1, edgePos2, Color.Transparent);
                        }
                    }
            }

        }

        public static void DrawAllLights()
        {
            for (int i = 0; i < lightNum; i++)
            {
                Light l = lights[i];

                Drawing.Draw(Engine.LightsRenderTarget, l.WorldPosition - new Vector2(MaxLightSize) / 2, new Rectangle(l.RenderTargetPosition.ToPoint(), new Point(MaxLightSize, MaxLightSize)), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None);
            }

            lightNum = 0;
        }
    }
}
