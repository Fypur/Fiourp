using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class Grid : Solid
    {
        public int[,] Organisation { get => Organisation; set => Organisation = value; }
        public Sprite[,] Tiles;

        public Grid(Vector2 position, int gridWidth, int gridHeight, int[,] org, Sprite[,] tiles = null) : base(position, gridWidth * tiles.GetLength(1), gridHeight * tiles.GetLength(0), Sprite.None)
        {
            Tiles = tiles;
            int[,] organisation = new int[tiles.GetLength(1), tiles.GetLength(0)];
            for(int x = 0; x < organisation.GetLength(1); x++)
                for(int y = 0; y < organisation.GetLength(0); y++)
                {
                    if (tiles[x, y] != null && tiles[x, y] != Sprite.None)
                        organisation[y, x] = 1;
                }

            RemoveComponent(Collider);
            Collider = new GridCollider(Vector2.Zero, gridWidth, gridHeight, org);
            AddComponent(Collider);
        }
        public override void Render()
        {
            base.Render();

            var gridCol = (GridCollider)Collider;
            Vector2 startPos = (Engine.Cam.Pos - Pos) / gridCol.GridSize;

            if (startPos.X > Width || startPos.Y > Height || startPos.X + Engine.RenderTarget.Width < 0 || startPos.Y + Engine.RenderTarget.Height < 0)
                return;

            Vector2 size = Engine.Cam.Size / gridCol.GridSize;

            for(int x = (int)startPos.X; x < startPos.X + size.X; x++)
            {
                for(int y = (int)startPos.Y; y < startPos.Y + size.Y; y++)
                {
                    Vector2 pos = new Vector2(x * gridCol.GridWidth, y * gridCol.GridHeight) + Collider.AbsolutePosition;
                    if(Tiles[y, x] != Sprite.None && Tiles[y, x] != null)
                        Tiles[y, x].Draw(pos);
                }
            }
        }
    }
}
