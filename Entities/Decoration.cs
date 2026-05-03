using Microsoft.Xna.Framework;

namespace Fiourp
{
    public class Decoration : Entity
    {
        public Decoration(Vector2 position, int width, int height, Sprite sprite) : base(position, width, height, sprite)
        {
            RemoveComponent(Collider);
            Collider = null;
        }
    }
}
