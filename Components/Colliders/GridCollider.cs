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
        public int[,] Organisation;

        private AABBCollider box;

        public override float Left { get => LocalPos.X; set => LocalPos.X = value; }
        public override float Right { get => LocalPos.X + Width; set => LocalPos.X = value - Width; }
        public override float Top { get => LocalPos.Y; set => LocalPos.Y = value; }
        public override float Bottom { get => LocalPos.Y + Height; set => LocalPos.Y = value - Height; }

        public GridCollider(Vector2 localPosition, int gridWidth, int gridHeight, int[,] organisation)
        {
            LocalPos = localPosition;
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            Organisation = organisation;
        }

        public override void Added()
        {
            base.Added();

            box = (AABBCollider)ParentEntity.AddComponent(new AABBCollider(LocalPos, GridWidth, GridHeight));
            box.Collidable = false;
        }

        public override bool Collide(Vector2 point)
        {
            point = point - WorldPos;
            if(point.X < 0 || point.Y < 0 || point.X >= Width || point.Y >= Height)
                return false;

            Point gridPoint = (point / new Vector2(GridWidth, GridHeight)).ToPoint();
            return Organisation[gridPoint.Y, gridPoint.X] == 1;
        }

        public override bool Collide(AABBCollider other)
        {
            Vector2 relativePos = other.WorldPos - WorldPos;
            if (relativePos.X + other.Width < 0 || relativePos.Y + other.Height < 0 || relativePos.X >= Width || relativePos.Y >= Height)
                return false;

            //Debug.PointUpdate(relativePos);
            Vector2 gridPos = relativePos / new Vector2(GridWidth, GridHeight);

            for(int x = (int)gridPos.X; x < gridPos.X + other.Width / GridWidth; x++)
            {
                for(int y = (int)gridPos.Y; y < gridPos.Y + other.Height / GridHeight; y++)
                {
                    //Debug.LogUpdate(new Vector2(x, y));
                    if (x < 0 || y < 0 || x >= Organisation.GetLength(1) || y >= Organisation.GetLength(0))
                        continue;

                    if (Organisation[y, x] == 1)
                        return true;
                }
            }

            return false;
        }

        public override bool Collide(BoxCollider other)
        {
            Vector2 relativePos = new Vector2(other.AbsoluteLeft, other.AbsoluteTop) - WorldPos;
            if (relativePos.X + other.Width < 0 || relativePos.Y + other.Height < 0 || relativePos.X >= Width || relativePos.Y >= Height)
                return false;

            //Debug.PointUpdate(relativePos);
            Vector2 gridPos = relativePos / new Vector2(GridWidth, GridHeight);

            for (int x = (int)gridPos.X; x < gridPos.X + other.Width / GridWidth; x++)
            {
                for (int y = (int)gridPos.Y; y < gridPos.Y + other.Height / GridHeight; y++)
                {
                    //Debug.LogUpdate(new Vector2(x, y));
                    if (x < 0 || y < 0 || x >= Organisation.GetLength(1) || y >= Organisation.GetLength(0))
                        continue;


                    if (Organisation[y, x] == 1)
                    {
                        box.LocalPos = LocalPos + new Vector2(x * GridWidth, y * GridHeight);
                        if(other.Collide(box))
                            return true;
                    }
                }
            }

            return false;
        }

        public override bool Collide(CircleCollider other)
        {
            return false;
            throw new NotImplementedException();
        }

        public override bool Collide(GridCollider other)
        {
            throw new NotImplementedException();
        }
    }
}
