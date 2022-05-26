using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class UIImage : UIElement
    {
        public UIImage(Vector2 position, int width, int height, bool centered, Sprite sprite, List<UIElement> children) : base(position, width, height, centered, sprite, children)
        { }
    }
}
