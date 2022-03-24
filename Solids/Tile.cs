using Microsoft.Xna.Framework;

namespace Fiourp
{
    public class Tile : Entity
    {
        public Tile(Vector2 position, int width, int height, Sprite sprite) : base(position, width, height, sprite)
        {
            Collider = null;
            RemoveComponent(Collider);
            Debug.Log(sprite);
        }
    }
}