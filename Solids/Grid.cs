using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp.Solids
{
    public class Grid : Solid
    {
        public Sprite[,] Tiles;
        public Grid(Vector2 position, int gridWidth, int gridHeight, Sprite[,] tiles) : base(position, gridWidth * tiles.GetLength(1), gridHeight * tiles.GetLength(0), Sprite.None)
        {
            Tiles = tiles;
            int[,] organisation = new int[tiles.GetLength(0), tiles.GetLength(1)];
            for(int x = 0; x < organisation.GetLength(1); x++)
                for(int y = 0; y < organisation.GetLength(0); y++)
                {
                    if (tiles[x, y] != null && tiles[x, y] != Sprite.None)
                        organisation[y, x] = 1;
                }

            Collider = new GridCollider(Vector2.Zero, gridWidth, gridHeight, organisation);
        }
    }
}
