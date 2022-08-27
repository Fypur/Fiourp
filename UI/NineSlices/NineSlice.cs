using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public abstract class NineSlice
    {
        protected abstract Texture2D TopLeftTile { get; }
        protected abstract Texture2D TopRightTile { get; }
        protected abstract Texture2D BottomRightTile { get; }
        protected abstract Texture2D BottomLeftTile { get; }
        protected abstract Texture2D TopTile { get; }
        protected abstract Texture2D LeftTile { get; }
        protected abstract Texture2D RightTile { get; }
        protected abstract Texture2D BottomTile { get; }
        protected abstract Texture2D FillTile { get; }

        public bool Repeat;

        public override string ToString()
            => $"Corners: {TopLeftTile.Name}, {TopRightTile.Name}, {BottomLeftTile.Name}, {BottomRightTile.Name}, Sides: {TopTile.Name}, {LeftTile.Name}, {RightTile.Name}, {BottomTile.Name}, Fill: {FillTile.Name}";

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(TopLeftTile);
            hash.Add(TopRightTile);
            hash.Add(BottomRightTile);
            hash.Add(BottomLeftTile);
            hash.Add(TopTile);
            hash.Add(LeftTile);
            hash.Add(RightTile);
            hash.Add(BottomTile);
            hash.Add(FillTile);
            hash.Add(Repeat);
            return hash.ToHashCode();
        }

        public virtual void Draw(Sprite sprite)
        {
            DrawSlice(TopLeftTile, sprite.ParentEntity.Pos, Size(TopLeftTile));
            DrawSlice(TopRightTile, sprite.ParentEntity.Pos + new Vector2(sprite.ParentEntity.Width - Size(TopRightTile).X, 0), Size(TopRightTile));
            DrawSlice(BottomLeftTile, sprite.ParentEntity.Pos + new Vector2(0, sprite.ParentEntity.Height - Size(BottomLeftTile).Y), Size(BottomLeftTile));
            DrawSlice(BottomRightTile, sprite.ParentEntity.Pos + new Vector2(sprite.ParentEntity.Width - Size(BottomRightTile).X, sprite.ParentEntity.Height - Size(BottomRightTile).Y), Size(BottomRightTile));

            if (Repeat)
            {
                if (TopTile != null)
                    for (int i = 0; i < sprite.ParentEntity.Width - Size(TopLeftTile).X - Size(TopRightTile).X; i += Size(TopTile).X)
                        DrawSlice(TopTile, sprite.ParentEntity.Pos + new Vector2(Size(TopLeftTile).X + i, 0), Size(TopTile));

                if (BottomTile != null)
                    for (int i = 0; i < sprite.ParentEntity.Width - Size(BottomLeftTile).X - Size(BottomRightTile).X; i += Size(BottomTile).X)
                        DrawSlice(BottomTile, sprite.ParentEntity.Pos + new Vector2(Size(BottomLeftTile).X + i, sprite.ParentEntity.Height - Size(BottomTile).Y), Size(BottomTile));

                if (RightTile != null)
                    for (int i = 0; i < sprite.ParentEntity.Height - Size(TopRightTile).Y - Size(BottomRightTile).Y; i += Size(RightTile).Y)
                        DrawSlice(RightTile, sprite.ParentEntity.Pos + new Vector2(sprite.ParentEntity.Width - Size(TopRightTile).X, Size(TopRightTile).Y + i), Size(RightTile));

                if (LeftTile != null)
                    for (int i = 0; i < sprite.ParentEntity.Height - Size(TopLeftTile).Y - Size(BottomLeftTile).Y; i += Size(LeftTile).Y)
                        DrawSlice(LeftTile, sprite.ParentEntity.Pos + new Vector2(0, Size(TopLeftTile).Y + i), Size(LeftTile));

                if (FillTile != null)
                    for (int x = Size(TopLeftTile).X; x <= sprite.ParentEntity.Width - Size(TopLeftTile).X - Size(BottomRightTile).X; x += Size(FillTile).X)
                        for (int y = Size(TopLeftTile).Y; y <= sprite.ParentEntity.Height - Size(TopLeftTile).Y - Size(BottomRightTile).Y; y += Size(FillTile).Y)
                            DrawSlice(FillTile, sprite.ParentEntity.Pos + new Vector2(x, y),
                                new Point(Math.Min(Size(FillTile).X, sprite.ParentEntity.Width - Size(RightTile).X - x), Math.Min(Size(FillTile).Y, sprite.ParentEntity.Height - Size(BottomTile).Y - y)));

            }
            else
            {
                DrawSlice(TopTile, sprite.ParentEntity.Pos + new Vector2(Size(TopLeftTile).X, 0), new Point(sprite.ParentEntity.Width - Size(TopLeftTile).X - Size(TopRightTile).X, Size(TopTile).Y));
                DrawSlice(BottomTile, sprite.ParentEntity.Pos + new Vector2(Size(BottomLeftTile).X, sprite.ParentEntity.Height - Size(BottomTile).Y), new Point(sprite.ParentEntity.Width - Size(BottomLeftTile).X - Size(BottomRightTile).X, Size(BottomTile).Y));
                DrawSlice(RightTile, sprite.ParentEntity.Pos + new Vector2(sprite.ParentEntity.Width - Size(TopRightTile).X, Size(TopRightTile).Y), new Point(Size(RightTile).X, sprite.ParentEntity.Height - Size(TopRightTile).Y - Size(BottomRightTile).Y));
                DrawSlice(LeftTile, sprite.ParentEntity.Pos + new Vector2(0, Size(TopLeftTile).Y), new Point(Size(LeftTile).X, sprite.ParentEntity.Height - Size(TopLeftTile).Y - Size(BottomLeftTile).Y));

                DrawSlice(FillTile, sprite.ParentEntity.Pos + Size(TopLeftTile).ToVector2(), sprite.ParentEntity.Size.ToPoint() - Size(TopLeftTile) - Size(BottomRightTile));
            }

            return;

            static Point Size(Texture2D texture) => texture != null ? new Point(texture.Width, texture.Height) : Point.Zero;

            void DrawSlice(Texture2D texture, Vector2 position, Point size)
            {
                if (texture != null)
                    Drawing.Draw(texture, position + sprite.Offset, size.ToVector2() * sprite.Scale, sprite.Color, sprite.Rotation, sprite.Origin, sprite.Effect, sprite.LayerDepth);
                /*Drawing.Draw
            }Edge(new Rectangle(position.ToPoint(), size.ToPoint()), 1, new Color(){ R = 255, G = 255, B = 255, A = 50 });*/
            }
        }
    }
}
