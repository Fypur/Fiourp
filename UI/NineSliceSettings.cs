using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public struct NineSliceSettings
    {
        public Texture2D TopLeft, TopRight, BottomRight, BottomLeft;
        public Texture2D Top, Left, Right, Bottom;
        public Texture2D Fill;

        public bool Repeat;

        public static readonly NineSliceSettings Empty = new NineSliceSettings();

        public NineSliceSettings(Texture2D topLeft, Texture2D topRight, Texture2D bottomLeft, Texture2D bottomRight,
            Texture2D top, Texture2D left, Texture2D right, Texture2D bottom, Texture2D fill, bool repeat) 
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            Top = top;
            Left = left;
            Right = right;
            Bottom = bottom;
            Fill = fill;
            Repeat = repeat;
        }

        public NineSliceSettings(Texture2D corners, Texture2D top, Texture2D fill, bool repeat)
        {
            TopLeft = corners;
            TopRight = corners.FlipX();
            BottomLeft = corners.FlipY();
            BottomRight = corners.FlipXAndY();

            Top = top;
            Right = top.Rotate90();
            Left = Right.FlipX();
            Bottom = top.FlipY();

            Fill = fill;

            Repeat = repeat;
        }

        public static bool operator ==(NineSliceSettings value, NineSliceSettings other)
            => value.TopRight == other.TopRight && value.TopLeft == other.TopLeft && value.BottomLeft == other.BottomLeft && value.BottomRight == other.BottomRight && value.Top == other.Top && value.Bottom == other.Bottom && value.Left == other.Left && value.Right == other.Right && value.Fill == other.Fill;

        public static bool operator !=(NineSliceSettings value, NineSliceSettings other)
            => !(value == other);

        public override bool Equals(object obj)
            => (NineSliceSettings)obj == this;

        public override string ToString()
            => $"Corners: {TopLeft.Name}, {TopRight.Name}, {BottomLeft.Name}, {BottomRight.Name}, Sides: {Top.Name}, {Left.Name}, {Right.Name}, {Bottom.Name}, Fill: {Fill.Name}";

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(TopLeft);
            hash.Add(TopRight);
            hash.Add(BottomRight);
            hash.Add(BottomLeft);
            hash.Add(Top);
            hash.Add(Left);
            hash.Add(Right);
            hash.Add(Bottom);
            hash.Add(Fill);
            hash.Add(Repeat);
            return hash.ToHashCode();
        }
    }
}
