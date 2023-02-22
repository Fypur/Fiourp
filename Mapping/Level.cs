using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Fiourp
{
    public class Level
    {
        public int TileWidth = 8;
        public int TileHeight = 8;
        public int ChunkSize = 8;

        public readonly Vector2 Pos;

        public readonly Vector2 Size;
        public readonly int[,] Organisation;
        public readonly Vector2[] Corners;
        public readonly Vector2[] InsideCorners;
        public readonly List<int[]> Edges;
        public readonly List<int[]>[,] ChunksEdge;

        public readonly Map ParentMap;

        public List<Entity> EntityData;
        public List<Entity> DontDestroyOnUnloadEntities;
        public Action EnterAction = null;

        public Level(LevelData data)
        {
            Pos = data.Pos;
            ParentMap = data.ParentMap;

            Organisation = data.Organisation;
            Size = data.Size;
            Corners = GetLevelCorners();
            InsideCorners = GetLevelInsideCorners();

            ChunksEdge = new List<int[]>[(int)Math.Ceiling(Organisation.GetLength(0) / (float)ChunkSize), (int)Math.Ceiling(Organisation.GetLength(1) / (float)ChunkSize)];
            Edges = GetEdges();

            EntityData = data.Entities;
            if (EntityData == null)
                EntityData = new List<Entity>();
            EnterAction = data.EnterAction;

            DontDestroyOnUnloadEntities = new();
        }

        public void Load()
        {
            Engine.CurrentMap.CurrentLevel = this;
            EnterAction?.Invoke();

            for (int y = 0; y < Organisation.GetLength(0); y++)
            {
                for (int x = 0; x < Organisation.GetLength(1); x++)
                {
                    if (Organisation[y, x] != 0)
                    {
                        EntityData.Add(new SolidTile(GetTileTexture(x, y), new Vector2(Pos.X + x * TileWidth, Pos.Y + y * TileHeight), TileWidth, TileHeight));
                    }
                }
            }

            foreach (Entity e in EntityData)
            {
                ParentMap.Instantiate(e);
            }
        }

        /// <summary>
        /// Use this when tiles are already present in entityData
        /// </summary>
        public void LoadNoAutoTile()
        {
            Engine.CurrentMap.CurrentLevel = this;
            EnterAction?.Invoke();

            for(int i = EntityData.Count - 1; i >= 0; i--)
            {
                ParentMap.Instantiate(EntityData[i]);
            }
        }

        public void Unload()
        {
            if (Engine.CurrentMap.CurrentLevel == this)
                Engine.CurrentMap.CurrentLevel = null;

            for (int i = EntityData.Count - 1; i >= 0; i--)
            {
                ParentMap.Destroy(EntityData[i]);
            }
        }

        public Vector2[] GetLevelCorners()
        {
            List<Vector2> points = new List<Vector2>();
            for (int x = 0; x < Organisation.GetLength(1); x++)
            {
                for (int y = 0; y < Organisation.GetLength(0); y++)
                {
                    if (Organisation[y, x] != 0)
                    {
                        if (GetOrganisation(x - 1, y) == 0 && GetOrganisation(x, y - 1) == 0 && GetOrganisation(x - 1, y - 1) == 0)
                            points.Add(Pos + new Vector2(x * TileWidth, y * TileHeight));

                        if (GetOrganisation(x + 1, y) == 0 && GetOrganisation(x, y - 1) == 0 && GetOrganisation(x + 1, y - 1) == 0)
                            points.Add(Pos + new Vector2((x + 1) * TileWidth, y * TileHeight));

                        if (GetOrganisation(x - 1, y) == 0 && GetOrganisation(x, y + 1) == 0 && GetOrganisation(x - 1, y + 1) == 0)
                            points.Add(Pos + new Vector2(x * TileWidth, (y + 1) * TileHeight));

                        if (GetOrganisation(x + 1, y) == 0 && GetOrganisation(x, y + 1) == 0 && GetOrganisation(x + 1, y + 1) == 0)
                            points.Add(Pos + new Vector2((x + 1) * TileWidth, (y + 1) * TileHeight));
                    }
                }
            }

            return points.ToArray();
        }

        public Vector2[] GetLevelInsideCorners()
        {
            List<Vector2> points = new List<Vector2>();
            for (int x = 0; x < Organisation.GetLength(1); x++)
            {
                for (int y = 0; y < Organisation.GetLength(0); y++)
                {
                    if (Organisation[y, x] != 0)
                    {
                        if (GetOrganisation(x - 1, y) != 0 && GetOrganisation(x, y - 1) != 0 && GetOrganisation(x - 1, y - 1) == 0)
                            points.Add(Pos + new Vector2(x * TileWidth, y * TileHeight));

                        if (GetOrganisation(x + 1, y) != 0 && GetOrganisation(x, y - 1) != 0 && GetOrganisation(x + 1, y - 1) == 0)
                            points.Add(Pos + new Vector2((x + 1) * TileWidth, y * TileHeight));

                        if (GetOrganisation(x - 1, y) != 0 && GetOrganisation(x, y + 1) != 0 && GetOrganisation(x - 1, y + 1) == 0)
                            points.Add(Pos + new Vector2(x * TileWidth, (y + 1) * TileHeight));

                        if (GetOrganisation(x + 1, y) != 0 && GetOrganisation(x, y + 1) != 0 && GetOrganisation(x + 1, y + 1) == 0)
                            points.Add(Pos + new Vector2((x + 1) * TileWidth, (y + 1) * TileHeight));

                        //Corners that are inside corners
                        if (GetOrganisation(x - 1, y) == 0 && GetOrganisation(x, y - 1) == 0 && GetOrganisation(x - 1, y - 1) != 0)
                            points.Add(Pos + new Vector2(x * TileWidth, y * TileHeight));

                        if (GetOrganisation(x + 1, y) == 0 && GetOrganisation(x, y - 1) == 0 && GetOrganisation(x + 1, y - 1) != 0)
                            points.Add(Pos + new Vector2((x + 1) * TileWidth, y * TileHeight));

                        if (GetOrganisation(x - 1, y) == 0 && GetOrganisation(x, y + 1) == 0 && GetOrganisation(x - 1, y + 1) != 0)
                            points.Add(Pos + new Vector2(x * TileWidth, (y + 1) * TileHeight));

                        if (GetOrganisation(x + 1, y) == 0 && GetOrganisation(x, y + 1) == 0 && GetOrganisation(x + 1, y + 1) != 0)
                            points.Add(Pos + new Vector2((x + 1) * TileWidth, (y + 1) * TileHeight));
                    }
                }
            }

            return points.ToArray();
        }

        public void DestroyOnUnload(Entity entity)
        {
            EntityData.Add(entity);
            DontDestroyOnUnloadEntities.Remove(entity);
        }
        public void DontDestroyOnUnload(Entity entity)
        {
            EntityData.Remove(entity);
            DontDestroyOnUnloadEntities.Add(entity);
        }

        public int GetOrganisation(int x, int y, int returnIfEmpty = 0)
        {
            if (x >= 0 && x < Organisation.GetLength(1) && y >= 0 && y < Organisation.GetLength(0))
                return Organisation[y, x];
            else
                return returnIfEmpty;
        }

        private Texture2D GetTileTexture(int x, int y)
        {
            int tileValue = Organisation[y, x];
            Dictionary<string, Texture2D> tileSet = DataManager.Tilesets[tileValue];

            bool rightBlock = GetOrganisation(x + 1, y, tileValue) == tileValue;
            bool leftBlock = GetOrganisation(x - 1, y, tileValue) == tileValue;
            bool topBlock = GetOrganisation(x, y - 1, tileValue) == tileValue;
            bool bottomBlock = GetOrganisation(x, y + 1, tileValue) == tileValue;

            bool topRightBlock = GetOrganisation(x + 1, y - 1, tileValue) == tileValue;
            bool topLeftBlock = GetOrganisation(x - 1, y - 1, tileValue) == tileValue;
            bool bottomRightBlock = GetOrganisation(x + 1, y + 1, tileValue) == tileValue;
            bool bottomLeftBlock = GetOrganisation(x - 1, y - 1, tileValue) == tileValue;

            #region testing each block

            if (!leftBlock && !rightBlock && !topBlock && !bottomBlock)
                return tileSet["complete"];

            if (!topBlock && rightBlock && leftBlock && bottomBlock)
                return tileSet["top"];

            if (!bottomBlock && rightBlock && leftBlock && topBlock)
                return tileSet["bottom"];

            if (topBlock && bottomBlock && !rightBlock && leftBlock)
                return tileSet["right"];

            if (topBlock && bottomBlock && rightBlock && !leftBlock)
                return tileSet["left"];

            if (!topBlock && rightBlock && !leftBlock && !bottomBlock)
                return tileSet["leftFullCorner"];

            if (!topBlock && !rightBlock && leftBlock && !bottomBlock)
                return tileSet["rightFullCorner"];

            if (!topBlock && !rightBlock && !leftBlock && bottomBlock)
                return tileSet["upFullCorner"];

            if (topBlock && !rightBlock && !leftBlock && !bottomBlock)
                return tileSet["downFullCorner"];

            if (!topBlock && leftBlock && rightBlock && !bottomBlock)
                return tileSet["horizontalPillar"];

            if (topBlock && !leftBlock && !rightBlock && bottomBlock)
                return tileSet["verticalPillar"];

            if (!topBlock && !rightBlock)
                return tileSet["topRightCorner"];

            if (!topBlock && !leftBlock)
                return tileSet["topLeftCorner"];

            if (!bottomBlock && !rightBlock)
                return tileSet["bottomRightCorner"];

            if (!bottomBlock && !leftBlock)
                return tileSet["bottomLeftCorner"];

            if (topBlock && rightBlock && !topRightBlock)
                return tileSet["topRightPoint"];

            if (topBlock && leftBlock && !topLeftBlock)
                return tileSet["topLeftPoint"];

            if (bottomBlock && rightBlock && !bottomRightBlock)
                return tileSet["bottomRightPoint"];

            if (bottomBlock && leftBlock && !bottomLeftBlock)
                return tileSet["bottomLeftPoint"];

            #endregion

            return tileSet["inside"];
        }

        public override string ToString()
            => $"Position: {Pos} \nSize: {Size}";

        public bool Contains(Vector2 point)
            => new Rectangle(Pos.ToPoint(), Size.ToPoint()).Contains(point);

        public Vector2 ToTileCoordinates(Vector2 position)
            => new Vector2((float)Math.Floor(position.X / TileWidth) * TileWidth,
                (float)Math.Floor(position.Y / TileWidth) * TileHeight);

        public Vector2 ToClosestTileCoordinates(Vector2 position)
            => new Vector2((float)Math.Round(position.X / TileWidth) * TileWidth,
                (float)Math.Round(position.Y / TileWidth) * TileHeight);

        /*public List<int[]> GetEdges()
        {
            List<int[]> edges = new();
            for (int y = 0; y < Engine.CurrentMap.CurrentLevel.Organisation.GetLength(0); y++)
                for (int x = 0; x < Engine.CurrentMap.CurrentLevel.Organisation.GetLength(1); x++)
                {
                    if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x, y) != 0)
                    {
                        if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x - 1, y) == 0)
                            edges.Add(new int[4] { x, y, x, y + 1 });
                        if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x + 1, y) == 0)
                        {
                            edges.Add(new int[4] { x + 1, y, x + 1, y + 1 });
                            //edges.Add(new int[4] { x + 1, y, x + 1, y + 1 });
                        }
                        if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x, y - 1) == 0)
                            edges.Add(new int[4] { x, y, x + 1, y });
                        if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x, y + 1) == 0)
                            edges.Add(new int[4] { x, y + 1, x + 1, y + 1 });
                    }
                }

            return edges;
        }*/

        public List<int[]> GetEdges()
        {
            List<int[]> edges = new();

            for (int y = 0; y < Organisation.GetLength(0); y++)
                for (int x = 0; x < Organisation.GetLength(1); x++)
                {
                    if(GetOrganisation(x, y) != 0)
                    {
                        //Top
                        if(GetOrganisation(x, y - 1) == 0 && (GetOrganisation(x - 1, y) == 0 || GetOrganisation(x - 1, y - 1) != 0))
                        {
                            int xMove = x + 1;
                            while(GetOrganisation(xMove, y) != 0 && GetOrganisation(xMove, y - 1) == 0)
                                xMove++;

                            edges.Add(new int[4] { x, y, xMove, y });
                        }

                        //Bottom
                        if (GetOrganisation(x, y + 1) == 0 && (GetOrganisation(x - 1, y) == 0 || GetOrganisation(x - 1, y + 1) != 0))
                        {
                            int xMove = x + 1;
                            while (GetOrganisation(xMove, y) != 0 && GetOrganisation(xMove, y + 1) == 0)
                                xMove++;

                            edges.Add(new int[4] { x, y + 1, xMove, y + 1 });
                        }

                        //Left
                        if (GetOrganisation(x - 1, y) == 0 && (GetOrganisation(x, y - 1) == 0 || GetOrganisation(x - 1, y - 1) != 0))
                        {
                            int yMove = y + 1;
                            while (GetOrganisation(x, yMove) != 0 && GetOrganisation(x - 1, yMove) == 0)
                                yMove++;

                            edges.Add(new int[4] { x, y, x, yMove });
                        }

                        //Right
                        if (GetOrganisation(x + 1, y) == 0 && (GetOrganisation(x, y - 1) == 0 || GetOrganisation(x + 1, y - 1) != 0))
                        {
                            int yMove = y + 1;
                            while (GetOrganisation(x, yMove) != 0 && GetOrganisation(x + 1, yMove) == 0)
                                yMove++;

                            edges.Add(new int[4] { x + 1, y, x + 1, yMove });
                        }
                    }
                }


            for (int x = 0; x < ChunksEdge.GetLength(1); x++)
                for (int y = 0; y < ChunksEdge.GetLength(0); y++)
                    ChunksEdge[y, x] = new List<int[]>();

            bool chunkdivisibleX = Organisation.GetLength(1) % ChunkSize == 0;
            bool chunkdivisibleY = Organisation.GetLength(0) % ChunkSize == 0;
            foreach (int[] edge in edges)
            {
                int chunk1x = edge[0] / ChunkSize + (edge[0] == Organisation.GetLength(1) && chunkdivisibleX ? -1 : 0);
                int chunk1y = edge[1] / ChunkSize + (edge[1] == Organisation.GetLength(0) && chunkdivisibleY ? -1 : 0);
                int chunk2x = edge[2] / ChunkSize + (edge[2] % ChunkSize == 0 && edge[2] != 0 ? -1 : 0);
                int chunk2y = edge[3] / ChunkSize + (edge[3] % ChunkSize == 0 && edge[3] != 0 ? -1 : 0);


                ChunksEdge[chunk1y, chunk1x].Add(edge);

                if (chunk1x != chunk2x)
                    for (int i = chunk1x + 1; i <= chunk2x; i++)
                        ChunksEdge[chunk1y, i].Add(edge);

                if (chunk1y != chunk2y)
                    for (int i = chunk1y + 1; i <= chunk2y; i++)
                        ChunksEdge[i, chunk1x].Add(edge);
            }

            /*foreach (int[] edge in ChunksEdge[1, 0])
                Debug.Point(Color.DarkOrange, new Vector2(edge[0], edge[1]) * 8 + Pos, new Vector2(edge[2], edge[3]) * 8 + Pos);*/

            return edges;
        }

        public Vector2 GetOrganisationPos(int x, int y)
            => new Vector2(x * TileWidth, y * TileHeight) + Pos;

        /*public void DebugEdge(int[] edge)
            => Debug.PointUpdate(Color.DarkOrange, new Vector2(edge[0], edge[1]) * 8 + Pos, new Vector2(edge[2], edge[3]) * 8 + Pos);*/
    }
}
