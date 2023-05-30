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

        private BoxCollider box;

        public override float Width { get => GridWidth * Organisation.GetLength(1); set => GridWidth = (int)(value / Organisation.GetLength(1)); }
        public override float Height { get => GridHeight * Organisation.GetLength(0); set => GridHeight = (int)(value / Organisation.GetLength(0)); }
        public override float Left { get => Pos.X; set => Pos.X = value; }
        public override float Right { get => Pos.X + Width; set => Pos.X = value - Width; }
        public override float Top { get => Pos.Y; set => Pos.Y = value; }
        public override float Bottom { get => Pos.Y + Height; set => Pos.Y = value - Height; }

        public GridCollider(Vector2 localPosition, int gridWidth, int gridHeight, int[,] organisation)
        {
            Pos = localPosition;
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            Organisation = organisation;
        }

        public override void Added()
        {
            base.Added();

            box = (BoxCollider)ParentEntity.AddComponent(new BoxCollider(Pos, GridWidth, GridHeight));
            box.Collidable = false;
        }

        public override bool Collide(Vector2 point)
        {
            point = point - AbsolutePosition;
            if(point.X < 0 || point.Y < 0 || point.X >= Width || point.Y >= Height)
                return false;

            Point gridPoint = (point / new Vector2(GridWidth, GridHeight)).ToPoint();
            return Organisation[gridPoint.Y, gridPoint.X] == 1;
        }

        public override bool Collide(BoxCollider other)
        {
            Vector2 relativePos = other.AbsolutePosition - AbsolutePosition;
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

        public override bool Collide(BoxColliderRotated other)
        {
            Vector2 relativePos = new Vector2(other.AbsoluteLeft, other.AbsoluteTop) - AbsolutePosition;
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
                        box.Pos = Pos + new Vector2(x * GridWidth, y * GridHeight);
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
