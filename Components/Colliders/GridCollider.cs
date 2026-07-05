using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Fiourp
{
    public class GridCollider : Collider
    {
        public Vector2 GridSize { get => new Vector2(TileWidth, TileHeight); set { TileWidth = (int)value.X; TileHeight = (int)value.Y; } }
        public int TileWidth;
        public int TileHeight;
        public int ChunkSize = 8;

        public bool[,] GridLayout;

        public int Width => TileWidth * GridLayout.GetLength(1);
        public int Height => TileHeight * GridLayout.GetLength(0);
        public override Rectangle Bounds => new Rectangle((int)WorldPos.X, (int)WorldPos.Y, Width, Height);

        public Vector2[] Corners { get; private set; }
        public Vector2[] InsideCorners { get; private set; }
        public List<int[]> Edges { get; private set; }
        public List<int[]>[,] ChunksEdge { get; private set; }


        private AABBCollider box;

        public GridCollider(Vector2 localPosition, int tileWidth, int tileHeight, bool[,] layout)
        {
            LocalPos = localPosition;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            GridLayout = layout;
        }

        public override void Added()
        {
            base.Added();

            Corners = GetLevelCorners();
            InsideCorners = GetLevelInsideCorners();

            ChunksEdge = new List<int[]>[(int)Math.Ceiling(GridLayout.GetLength(0) / (float)ChunkSize), (int)Math.Ceiling(GridLayout.GetLength(1) / (float)ChunkSize)];
            Edges = GetEdges();

            box = (AABBCollider)ParentEntity.AddComponent(new AABBCollider(LocalPos, TileWidth, TileHeight));
            box.Collidable = false;
        }

        private bool GeneralCollidingFunction(Rectangle bounds, Func<int, int, bool> checkingFunction)
        {
            Vector2 relativePos = bounds.Location.ToVector2() - WorldPos;

            if (relativePos.X < 0 || relativePos.Y < 0 || relativePos.X >= Width || relativePos.Y >= Height)
                return true;

            Vector2 gridPos = relativePos / new Vector2(TileWidth, TileHeight);

            for (int x = (int)gridPos.X; x < gridPos.X + (float)bounds.Width / TileWidth; x++)
            {
                for (int y = (int)gridPos.Y; y < gridPos.Y + (float)bounds.Height / TileHeight; y++)
                {
                    if (x < 0 || y < 0 || x >= GridLayout.GetLength(1) || y >= GridLayout.GetLength(0))
                        continue;

                    if (GridLayout[y, x] && checkingFunction(x, y))
                        return true;
                }
            }

            return false;
        }

        public override bool CollideRaw(Collider other)
        {
            if (other is AABBCollider aabb)
                return CollideRaw(aabb);

            return AABBBoxGridCollision(other);
        }

        public override bool Contains(Vector2 point)
        {
            point = point - WorldPos;
            if (point.X < 0 || point.Y < 0 || point.X >= Width || point.Y >= Height)
                return false;

            Point gridPoint = (point / new Vector2(TileWidth, TileHeight)).ToPoint();
            return GridLayout[gridPoint.Y, gridPoint.X];
        }

        private bool CollideRaw(AABBCollider other)
            => GeneralCollidingFunction(other.Bounds, (x, y) => true);

        private bool AABBBoxGridCollision(Collider collider)
        {
            return GeneralCollidingFunction(collider.Bounds, (x, y) =>
            {
                box.LocalPos = LocalPos + new Vector2(x * TileWidth, y * TileHeight);
                return collider.CollideRaw(box);
            });
        }

        private bool CollideRaw(BoxCollider other)
            => AABBBoxGridCollision(other);

        private bool CollideRaw(CircleCollider other)
            => AABBBoxGridCollision(other);

        public bool GetLayout(int x, int y, bool returnIfEmpty = false)
        {
            if (x >= 0 && x < GridLayout.GetLength(1) && y >= 0 && y < GridLayout.GetLength(0))
                return GridLayout[y, x];
            else
                return returnIfEmpty;
        }

        public Vector2[] GetLevelCorners()
        {
            List<Vector2> points = new List<Vector2>();
            for (int x = 0; x < GridLayout.GetLength(1); x++)
            {
                for (int y = 0; y < GridLayout.GetLength(0); y++)
                {
                    if (GridLayout[y, x])
                    {
                        if (!GetLayout(x - 1, y) && !GetLayout(x, y - 1) && !GetLayout(x - 1, y - 1))
                            points.Add(ParentEntity.Pos + new Vector2(x * TileWidth, y * TileHeight));

                        if (!GetLayout(x + 1, y) && !GetLayout(x, y - 1) && !GetLayout(x + 1, y - 1))
                            points.Add(ParentEntity.Pos + new Vector2((x + 1) * TileWidth, y * TileHeight));

                        if (!GetLayout(x - 1, y) && !GetLayout(x, y + 1) && !GetLayout(x - 1, y + 1))
                            points.Add(ParentEntity.Pos + new Vector2(x * TileWidth, (y + 1) * TileHeight));

                        if (!GetLayout(x + 1, y) && !GetLayout(x, y + 1) && !GetLayout(x + 1, y + 1))
                            points.Add(ParentEntity.Pos + new Vector2((x + 1) * TileWidth, (y + 1) * TileHeight));
                    }
                }
            }

            return points.ToArray();
        }

        public Vector2[] GetLevelInsideCorners()
        {
            List<Vector2> points = new List<Vector2>();
            for (int x = 0; x < GridLayout.GetLength(1); x++)
            {
                for (int y = 0; y < GridLayout.GetLength(0); y++)
                {
                    if (GridLayout[y, x])
                    {
                        if (!GetLayout(x - 1, y) && !GetLayout(x, y - 1) && GetLayout(x - 1, y - 1))
                            points.Add(ParentEntity.Pos + new Vector2(x * TileWidth, y * TileHeight));

                        if (!GetLayout(x + 1, y) && !GetLayout(x, y - 1) && GetLayout(x + 1, y - 1))
                            points.Add(ParentEntity.Pos + new Vector2((x + 1) * TileWidth, y * TileHeight));

                        if (GetLayout(x - 1, y) && GetLayout(x, y + 1) && !GetLayout(x - 1, y + 1))
                            points.Add(ParentEntity.Pos + new Vector2(x * TileWidth, (y + 1) * TileHeight));

                        if (GetLayout(x + 1, y) && GetLayout(x, y + 1) && !GetLayout(x + 1, y + 1))
                            points.Add(ParentEntity.Pos + new Vector2((x + 1) * TileWidth, (y + 1) * TileHeight));

                        //Corners that are inside corners
                        if (!GetLayout(x - 1, y) && !GetLayout(x, y - 1) && GetLayout(x - 1, y - 1))
                            points.Add(ParentEntity.Pos + new Vector2(x * TileWidth, y * TileHeight));

                        if (!GetLayout(x + 1, y) && !GetLayout(x, y - 1) && GetLayout(x + 1, y - 1))
                            points.Add(ParentEntity.Pos + new Vector2((x + 1) * TileWidth, y * TileHeight));

                        if (!GetLayout(x - 1, y) && !GetLayout(x, y + 1) && GetLayout(x - 1, y + 1))
                            points.Add(ParentEntity.Pos + new Vector2(x * TileWidth, (y + 1) * TileHeight));

                        if (!GetLayout(x + 1, y) && !GetLayout(x, y + 1) && GetLayout(x + 1, y + 1))
                            points.Add(ParentEntity.Pos + new Vector2((x + 1) * TileWidth, (y + 1) * TileHeight));
                    }
                }
            }

            return points.ToArray();
        }

        public List<int[]> GetEdges()
        {
            List<int[]> edges = new();

            for (int y = 0; y < GridLayout.GetLength(0); y++)
                for (int x = 0; x < GridLayout.GetLength(1); x++)
                {
                    if (GetLayout(x, y))
                    {
                        //Top
                        if (!GetLayout(x, y - 1) && (!GetLayout(x - 1, y) || GetLayout(x - 1, y - 1)))
                        {
                            int xMove = x + 1;
                            while (GetLayout(xMove, y) && !GetLayout(xMove, y - 1))
                                xMove++;

                            edges.Add(new int[4] { x, y, xMove, y });
                        }

                        //Bottom
                        if (!GetLayout(x, y + 1) && (!GetLayout(x - 1, y) || GetLayout(x - 1, y + 1)))
                        {
                            int xMove = x + 1;
                            while (GetLayout(xMove, y) && !GetLayout(xMove, y + 1))
                                xMove++;

                            edges.Add(new int[4] { x, y + 1, xMove, y + 1 });
                        }

                        //Left
                        if (!GetLayout(x - 1, y) && (!GetLayout(x, y - 1) || GetLayout(x - 1, y - 1)))
                        {
                            int yMove = y + 1;
                            while (GetLayout(x, yMove) && !GetLayout(x - 1, yMove))
                                yMove++;

                            edges.Add(new int[4] { x, y, x, yMove });
                        }

                        //Right
                        if (!GetLayout(x + 1, y) && (!GetLayout(x, y - 1) || GetLayout(x + 1, y - 1)))
                        {
                            int yMove = y + 1;
                            while (GetLayout(x, yMove) && !GetLayout(x + 1, yMove))
                                yMove++;

                            edges.Add(new int[4] { x + 1, y, x + 1, yMove });
                        }
                    }
                }


            for (int x = 0; x < ChunksEdge.GetLength(1); x++)
                for (int y = 0; y < ChunksEdge.GetLength(0); y++)
                    ChunksEdge[y, x] = new List<int[]>();

            bool chunkdivisibleX = GridLayout.GetLength(1) % ChunkSize == 0;
            bool chunkdivisibleY = GridLayout.GetLength(0) % ChunkSize == 0;
            foreach (int[] edge in edges)
            {
                int chunk1x = edge[0] / ChunkSize + (edge[0] == GridLayout.GetLength(1) && chunkdivisibleX ? -1 : 0);
                int chunk1y = edge[1] / ChunkSize + (edge[1] == GridLayout.GetLength(0) && chunkdivisibleY ? -1 : 0);
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
    }
}
