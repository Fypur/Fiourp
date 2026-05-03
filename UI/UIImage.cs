using Microsoft.Xna.Framework;

namespace Fiourp
{
    public class UIImage : UIElement
    {
        public UIImage(Vector2 position, int width, int height, bool centered, Sprite sprite) : base(position, width, height, centered, sprite)
        { selectableField = false; }
    }
}
