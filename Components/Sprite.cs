using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class Sprite : Renderer
    {
        public enum DrawMode { Centered, TopLeft, ScaledTopLeft }

        public float Width { get => Texture.Width; }
        public float Height { get => Texture.Height; }

        public Texture2D Texture;
        public float Rotation = 0;

        public Color Color = Color.White;
        public Vector2 Origin = Vector2.Zero;
        public Vector2 Scale = Vector2.One;
        public SpriteEffects Effect = SpriteEffects.None;
        public float LayerDepth = 0;

        public Rectangle? Rect = null;

        public override string ToString()
        {
            return $"Sprite: {Texture.Name}, {Color}, layerDepth: {LayerDepth}, Rect: {Rect}, Origin {Origin}, " +
                $"Scale: {Scale}, Rotation {Rotation}, SpriteEffects: {Effect}";
        }

        public Sprite(Texture2D texture)
        {
            Texture = texture;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
        }

        public Sprite(Texture2D texture, Vector2 origin)
        {
            Texture = texture;
            Origin = origin;
        }

        public Sprite(Color color)
        {
            Texture = Drawing.pointTexture;
            Color = color;
        }

        public Sprite(Color color, Rectangle? rect, float layerDepth = 0)
        {
            Texture = Drawing.pointTexture;
            Rect = rect;
            Color = color;
            LayerDepth = layerDepth;
        }

        public Sprite(Texture2D texture, Rectangle rect)
        {
            Texture = texture;
            Rect = rect;
        }

        public override void Render()
        {
            if (Texture == null)
                return;

            if (Rect is Rectangle rect)
                Drawing.Draw(Texture, rect, Color, Rotation, Origin, Scale, Effect, LayerDepth);
            else
            {
                Drawing.Draw(Texture, ParentEntity.Pos, null, Color, Rotation, Origin, Scale, Effect, LayerDepth);
            }
                
        }
    }
}
