using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public static class Collision
    {
        public static bool RectCircle(Rectangle rect, Vector2 cPosition, float cRadius)
        {
            Vector2 halfSize = new Vector2(rect.Width / 2, rect.Height / 2);

            float distX = Math.Abs(cPosition.X - (rect.Left + halfSize.X));
            float distY = Math.Abs(cPosition.Y - (rect.Top + halfSize.Y));

            if (distX >= cRadius + halfSize.X || distY >= cRadius + halfSize.Y)
                return false;
            if (distX < halfSize.X && distY < halfSize.Y)
                return true;

            distX -= halfSize.X;
            distY -= halfSize.Y;

            if(distX * distX + distY * distY < cRadius * cRadius)
                return true;

            return false;
        }
    }
}
