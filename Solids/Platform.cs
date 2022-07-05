using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class Platform : Entity
    {
        public Platform(Vector2 position, int width, int height, Sprite sprite) : base(position, width, height, sprite)
        { }
    }
}
