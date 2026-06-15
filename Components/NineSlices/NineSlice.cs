using Microsoft.Xna.Framework;
using System;

namespace Fiourp
{
    public abstract class NineSlice
    {
        protected abstract Sprite TopLeftTile { get; }
        protected abstract Sprite TopRightTile { get; }
        protected abstract Sprite BottomRightTile { get; }
        protected abstract Sprite BottomLeftTile { get; }
        protected abstract Sprite TopTile { get; }
        protected abstract Sprite LeftTile { get; }
        protected abstract Sprite RightTile { get; }
        protected abstract Sprite BottomTile { get; }
        protected abstract Sprite FillTile { get; }

        public bool Repeat;

        public override string ToString()
            => $"Corners: {TopLeftTile.Texture.Name}, {TopRightTile.Texture.Name}, {BottomLeftTile.Texture.Name}, {BottomRightTile.Texture.Name}, Sides: {TopTile.Texture.Name}, {LeftTile.Texture.Name}, {RightTile.Texture.Name}, {BottomTile.Texture.Name}, Fill: {FillTile.Texture.Name}";

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

        public virtual void Update()
        {
            TopLeftTile?.Update();
            TopRightTile?.Update();
            BottomRightTile?.Update();
            BottomLeftTile?.Update();
            TopTile?.Update();
            LeftTile?.Update();
            RightTile?.Update();
            BottomTile?.Update();
            FillTile?.Update();
        }

        public virtual void Draw(int width, int height, Sprite sprite)
        {
            static Point Size(Sprite texture) => texture != null ? new Point(texture.Width, texture.Height) : Point.Zero;

            void DrawSlice(Sprite texture, Vector2 position, Point size)
            {
                if (texture != null)
                    Drawing.Draw(texture.Texture, position + sprite.Offset, size.ToVector2() * sprite.Scale, sprite.Color, sprite.Rotation, sprite.Origin, sprite.SpriteEffect, sprite.LayerDepth);
            }

            DrawSlice(TopLeftTile, sprite.ParentEntity.Pos, Size(TopLeftTile));
            DrawSlice(TopRightTile, sprite.ParentEntity.Pos + new Vector2(width - Size(TopRightTile).X, 0), Size(TopRightTile));
            DrawSlice(BottomLeftTile, sprite.ParentEntity.Pos + new Vector2(0, height - Size(BottomLeftTile).Y), Size(BottomLeftTile));
            DrawSlice(BottomRightTile, sprite.ParentEntity.Pos + new Vector2(width - Size(BottomRightTile).X, height - Size(BottomRightTile).Y), Size(BottomRightTile));

            if (Repeat)
            {
                if (TopTile != null)
                    for (int i = 0; i < width - Size(TopLeftTile).X - Size(TopRightTile).X; i += Size(TopTile).X)
                        DrawSlice(TopTile, sprite.ParentEntity.Pos + new Vector2(Size(TopLeftTile).X + i, 0), Size(TopTile));

                if (BottomTile != null)
                    for (int i = 0; i < width - Size(BottomLeftTile).X - Size(BottomRightTile).X; i += Size(BottomTile).X)
                        DrawSlice(BottomTile, sprite.ParentEntity.Pos + new Vector2(Size(BottomLeftTile).X + i, height - Size(BottomTile).Y), Size(BottomTile));

                if (RightTile != null)
                    for (int i = 0; i < height - Size(TopRightTile).Y - Size(BottomRightTile).Y; i += Size(RightTile).Y)
                        DrawSlice(RightTile, sprite.ParentEntity.Pos + new Vector2(width - Size(TopRightTile).X, Size(TopRightTile).Y + i), Size(RightTile));

                if (LeftTile != null)
                    for (int i = 0; i < height - Size(TopLeftTile).Y - Size(BottomLeftTile).Y; i += Size(LeftTile).Y)
                        DrawSlice(LeftTile, sprite.ParentEntity.Pos + new Vector2(0, Size(TopLeftTile).Y + i), Size(LeftTile));

                if (FillTile != null)
                    for (int x = Size(TopLeftTile).X; x <= width - Size(TopLeftTile).X - Size(BottomRightTile).X; x += Size(FillTile).X)
                        for (int y = Size(TopLeftTile).Y; y <= height - Size(TopLeftTile).Y - Size(BottomRightTile).Y; y += Size(FillTile).Y)
                            DrawSlice(FillTile, sprite.ParentEntity.Pos + new Vector2(x, y),
                                new Point(Math.Min(Size(FillTile).X, width - Size(RightTile).X - x), Math.Min(Size(FillTile).Y, height - Size(BottomTile).Y - y)));

            }
            else
            {
                DrawSlice(TopTile, sprite.ParentEntity.Pos + new Vector2(Size(TopLeftTile).X, 0), new Point(width - Size(TopLeftTile).X - Size(TopRightTile).X, Size(TopTile).Y));
                DrawSlice(BottomTile, sprite.ParentEntity.Pos + new Vector2(Size(BottomLeftTile).X, height - Size(BottomTile).Y), new Point(width - Size(BottomLeftTile).X - Size(BottomRightTile).X, Size(BottomTile).Y));
                DrawSlice(RightTile, sprite.ParentEntity.Pos + new Vector2(width - Size(TopRightTile).X, Size(TopRightTile).Y), new Point(Size(RightTile).X, height - Size(TopRightTile).Y - Size(BottomRightTile).Y));
                DrawSlice(LeftTile, sprite.ParentEntity.Pos + new Vector2(0, Size(TopLeftTile).Y), new Point(Size(LeftTile).X, height - Size(TopLeftTile).Y - Size(BottomLeftTile).Y));

                DrawSlice(FillTile, sprite.ParentEntity.Pos + Size(TopLeftTile).ToVector2(), new Point(width, height) - Size(TopLeftTile) - Size(BottomRightTile));
            }

            return;
        }
    }
}
