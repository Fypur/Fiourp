using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class SolidPlatform : Solid
    {
        public SolidPlatform(Vector2 position, int width, int height, Sprite sprite) : base(position, width, height, sprite)
        {
        }

        public SolidPlatform(Vector2 position, int width, int height, Color color) : base(position, width, height, color)
        {
        }

        public SolidPlatform(Vector2 position, int width, int height, NineSlice nineSlice) : base(position, width, height, new Sprite(nineSlice))
        {
        }
    }
}
