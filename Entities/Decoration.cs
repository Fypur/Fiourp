using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class Decoration : Entity
    {
        public Decoration(Vector2 position, int width, int height, Sprite sprite) : base(position, width, height, sprite)
        {
            Collider = null;
            RemoveComponent(Collider);
        }
    }
}
