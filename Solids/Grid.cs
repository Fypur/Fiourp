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
        public int[,] Organisation { get => (Collider).Organisation; set => (Collider).Organisation = value; }
        public int TileWidth => Collider.GridWidth;
        public int TileHeight => Collider.GridHeight;

        public Sprite[,] Tiles;

        private new GridCollider Collider => base.Collider as GridCollider;

        public Grid(Vector2 position, int gridWidth, int gridHeight, int[,] org, Sprite[,] tiles = null) : base(position, gridWidth * tiles.GetLength(1), gridHeight * tiles.GetLength(0), Sprite.None)
        {
            Tiles = tiles;
            RemoveComponent(base.Collider);
            base.Collider = new GridCollider(Vector2.Zero, gridWidth, gridHeight, org);
            AddComponent(base.Collider);
        }
        public override void Render()
        {
            base.Render();

            var gridCol = Collider;
            Vector2 startPos = (Engine.Cam.Pos - Pos) / gridCol.GridSize;

            if (startPos.X > Width || startPos.Y > Height || startPos.X + Engine.RenderTarget.Width < 0 || startPos.Y + Engine.RenderTarget.Height < 0)
                return;

            

            Vector2 size = Engine.Cam.Size / gridCol.GridSize;

            for(int x = Math.Max((int)startPos.X, 0); x < startPos.X + size.X; x++)
            {
                for(int y = Math.Max((int)startPos.Y, 0); y < startPos.Y + size.Y; y++)
                {
                    Vector2 pos = new Vector2(x * gridCol.GridWidth, y * gridCol.GridHeight) + Collider.AbsolutePosition;
                    if(Tiles[y, x] != Sprite.None && Tiles[y, x] != null)
                        Tiles[y, x].Draw(pos);
                    if(Debug.DebugMode && Organisation[y, x] != 0)
                        Drawing.DrawEdge(new Rectangle(pos.ToPoint(), new Point(gridCol.GridWidth, gridCol.GridHeight)), 1, Color.Blue);
                    /*if (Organisation[y, x] != 0 && (Tiles[y, x] == null || Tiles[y, x] == Sprite.None))
                        Debug.LogUpdate(Organisation[y, x], Tiles[y, x], pos);
                    */
                }
            }
        }
    }
}
