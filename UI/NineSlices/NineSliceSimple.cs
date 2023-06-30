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
        public Sprite TopRight, TopLeft, BottomRight, BottomLeft, Top, Left, Right, Bottom, Fill;

        protected override Sprite TopLeftTile { get => TopLeft; }
        protected override Sprite TopRightTile { get => TopRight; }
        protected override Sprite BottomRightTile { get => BottomRight; }
        protected override Sprite BottomLeftTile { get => BottomLeft; }
        protected override Sprite TopTile { get => Top; }
        protected override Sprite LeftTile { get => Left; }
        protected override Sprite RightTile { get => Right; }
        protected override Sprite BottomTile { get => Bottom; }
        protected override Sprite FillTile { get => Fill; }

        public NineSliceSimple(Texture2D topLeft, Texture2D topRight, Texture2D bottomLeft, Texture2D bottomRight,
            Texture2D top, Texture2D left, Texture2D right, Texture2D bottom, Texture2D fill, bool repeat)
        {
            TopLeft = new Sprite(topLeft);
            TopRight = new Sprite(topRight);
            BottomLeft = new Sprite(bottomLeft);
            BottomRight = new Sprite(bottomRight);
            Top = new Sprite(top);
            Left = new Sprite(left);
            Right = new Sprite(right);
            Bottom = new Sprite(bottom);
            Fill = new Sprite(fill);

            Repeat = repeat;
        }

        public NineSliceSimple(Texture2D corners, Texture2D top, Texture2D fill, bool repeat)
        {
            TopLeft = new Sprite(corners);
            TopRight = new Sprite(corners.FlipX());
            BottomLeft = new Sprite(corners.FlipY());
            BottomRight = new Sprite(corners.FlipXAndY());

            Top = new Sprite(top);
            Right = new Sprite(top.Rotate90());
            Left = new Sprite(Right.Texture.FlipX());
            Bottom = new Sprite(top.FlipY());

            Fill = new Sprite(fill);

            Repeat = repeat;
        }
    }
}
