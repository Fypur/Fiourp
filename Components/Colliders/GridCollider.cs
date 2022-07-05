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
        public float GridWidth;
        public float GridHeight;
        public int[,] Organisation;

        public override float Width { get => GridWidth * Organisation.GetLength(1); set => GridWidth = value / Organisation.GetLength(1); }
        public override float Height { get => GridHeight * Organisation.GetLength(0); set => GridHeight = value / Organisation.GetLength(0); }
        public override float Left { get => Pos.X; set => Pos.X = value; }
        public override float Right { get => Pos.X + Width; set => Pos.X = value - Width; }
        public override float Top { get => Pos.Y; set => Pos.Y = value; }
        public override float Bottom { get => Pos.Y + Height; set => Pos.Y = value - Height; }

        public GridCollider(Vector2 localPosition, float gridWidth, float gridHeight, int[,] organisation)
        {
            Pos = localPosition;
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            Organisation = organisation;
        }

        public override bool Collide(Vector2 point)
        {
            point = point - AbsolutePosition;
            if(point.X < 0 || point.Y < 0 || point.X > Width || point.Y > Height)
                return false;

            Point gridPoint = (point / new Vector2(GridWidth, GridHeight)).ToPoint();
            return Organisation[gridPoint.X, gridPoint.Y] == 1;
        }

        public override bool Collide(BoxCollider other)
        {
            Vector2 relativePos = other.AbsolutePosition - AbsolutePosition;
            if (relativePos.X + other.Width < 0 || relativePos.Y + other.Height < 0 || relativePos.X > Width || relativePos.Y > Height)
                return false;

            for(int x = (int)relativePos.X; x < relativePos.X + other.Width; x++)
            {
                for(int y = (int)relativePos.Y; y < relativePos.Y + other.Height; y++)
                {
                    if(Collide(new Vector2(x, y)))
                        return true;
                }
            }

            return false;
        }

        public override bool Collide(CircleCollider other)
        {
            return false;
            throw new NotImplementedException();
        }
    }
}
