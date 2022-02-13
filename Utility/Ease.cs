using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public static class Ease
    {
        public static float QuintIn(float x)
            => (float)Math.Pow(x, 5);

        public static float QuintOut(float x)
            => 1 - (float) Math.Pow(1 - x, 5);

        public static float QuintInAndOut(float x)
            => x < 0.5f ? 4 * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 3) / 2;

        public static float None(float x) 
            => x;

        /// <summary>
        /// Make a value that goes from 1 to 0 go from 0 to 1 or the opposite
        /// </summary>
        /// <param name="t">value</param>
        /// <returns></returns>
        public static float Reverse(float t)
            => -t + 1;
    }
}