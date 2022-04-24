using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class NineSliceSettings
    {
        public Texture2D TopLeft, TopRight, BottomRight, BottomLeft;
        public Texture2D Top, Left, Right, Bottom;
        public Texture2D Fill;

        public NineSliceSettings(Texture2D topLeft, Texture2D topRight, Texture2D bottomLeft, Texture2D bottomRight,
            Texture2D top, Texture2D left, Texture2D right, Texture2D bottom, Texture2D fill) 
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
        }

        public NineSliceSettings(Texture2D corners, Texture2D top, Texture2D fill)
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
        }

        public override string ToString()
            => $"Corners: {TopLeft.Name}, {TopRight.Name}, {BottomLeft.Name}, {BottomRight.Name}, Sides: {Top.Name}, {Left.Name}, {Right.Name}, {Bottom.Name}, Fill: {Fill.Name}";
    }
}
