using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class GridCollider : Collider
    {
        public Vector2 GridSize { get => new Vector2(GridWidth, GridHeight); set { GridWidth = (int)value.X; GridHeight = (int)value.Y; } }
        public int GridWidth;
        public int GridHeight;
        public bool[,] Grid;

        private int Width { get => GridWidth * Grid.GetLength(1); }
        private int Height { get => GridHeight * Grid.GetLength(0); }
        public override Rectangle Bounds => new Rectangle((int)WorldPos.X, (int)WorldPos.Y, Width, Height);


        private AABBCollider box;

        public GridCollider(Vector2 localPosition, int gridWidth, int gridHeight, bool[,] organisation)
        {
            LocalPos = localPosition;
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            Grid = organisation;
        }

        public override void Added()
        {
            base.Added();

            box = (AABBCollider)ParentEntity.AddComponent(new AABBCollider(LocalPos, GridWidth, GridHeight));
            box.Collidable = false;
        }

        private bool GeneralCollidingFunction(Rectangle bounds, Func<int, int, bool> checkingFunction)
        {
            Vector2 relativePos = bounds.Location.ToVector2() - WorldPos;

            if (relativePos.X < 0 || relativePos.Y < 0 || relativePos.X >= Width || relativePos.Y >= Height)
                return true;

            Vector2 gridPos = relativePos / new Vector2(GridWidth, GridHeight);

            for (int x = (int)gridPos.X; x < gridPos.X + (float)bounds.Width / GridWidth; x++)
            {
                for (int y = (int)gridPos.Y; y < gridPos.Y + (float)bounds.Height / GridHeight; y++)
                {
                    if (x < 0 || y < 0 || x >= Grid.GetLength(1) || y >= Grid.GetLength(0))
                        continue;

                    if (Grid[y, x] && checkingFunction(x, y))
                        return true;
                }
            }

            return false;
        }

        public override bool CollideRaw(Collider other)
        {
            if(other is AABBCollider aabb)
                return CollideRaw(aabb);
            else if(other is BoxCollider box)
                return CollideRaw(box);
            else
                throw new NotImplementedException($"GridCollider - {other.GetType()} collision has not been implemented yet");
        }

        public override bool Contains(Vector2 point)
        {
            point = point - WorldPos;
            if(point.X < 0 || point.Y < 0 || point.X >= Width || point.Y >= Height)
                return false;

            Point gridPoint = (point / new Vector2(GridWidth, GridHeight)).ToPoint();
            return Grid[gridPoint.Y, gridPoint.X];
        }

        private bool CollideRaw(AABBCollider other)
            => GeneralCollidingFunction(other.Bounds, (x, y) => true);

        private bool AABBBoxGridCollision(Collider collider)
        {
            return GeneralCollidingFunction(collider.Bounds, (x, y) =>
            {
                box.LocalPos = LocalPos + new Vector2(x * GridWidth, y * GridHeight);
                return collider.CollideRaw(box);
            });
        }

        private bool CollideRaw(BoxCollider other)
            => AABBBoxGridCollision(other);

        private bool CollideRaw(CircleCollider other)
            => AABBBoxGridCollision(other);
    }
}
