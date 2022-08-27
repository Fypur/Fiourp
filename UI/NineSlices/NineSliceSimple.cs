using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class NineSliceSimple : NineSlice
    {
        public Texture2D TopRight, TopLeft, BottomRight, BottomLeft, Top, Left, Right, Bottom, Fill;

        protected override Texture2D TopLeftTile { get => TopLeft; }
        protected override Texture2D TopRightTile { get => TopRight; }
        protected override Texture2D BottomRightTile { get => BottomRight; }
        protected override Texture2D BottomLeftTile { get => BottomLeft; }
        protected override Texture2D TopTile { get => Top; }
        protected override Texture2D LeftTile { get => Left; }
        protected override Texture2D RightTile { get => Right; }
        protected override Texture2D BottomTile { get => Bottom; }
        protected override Texture2D FillTile { get => Fill; }

        public NineSliceSimple(Texture2D topLeft, Texture2D topRight, Texture2D bottomLeft, Texture2D bottomRight,
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

        public NineSliceSimple(Texture2D corners, Texture2D top, Texture2D fill, bool repeat)
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
    }
}
