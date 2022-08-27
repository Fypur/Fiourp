using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class NineSliceRandom : NineSlice
    {
        public List<Texture2D>
            TopRight = new(), TopLeft = new(), BottomRight = new(), BottomLeft = new(),
            Top = new(), Left = new(), Right = new(), Bottom = new(),
            Fill = new();

        private Random random;
        public int Seed;

        protected override Texture2D TopRightTile { get => GetRandomTile(TopRight); }
        protected override Texture2D TopLeftTile { get => GetRandomTile(TopLeft); }
        protected override Texture2D BottomRightTile { get => GetRandomTile(BottomRight); }
        protected override Texture2D BottomLeftTile { get => GetRandomTile(BottomLeft); }
        protected override Texture2D TopTile { get => GetRandomTile(Top); }
        protected override Texture2D LeftTile { get => GetRandomTile(Left); }
        protected override Texture2D RightTile { get => GetRandomTile(Right); }
        protected override Texture2D BottomTile { get => GetRandomTile(Bottom); }
        protected override Texture2D FillTile { get => GetRandomTile(Fill); }

        private Texture2D GetRandomTile(List<Texture2D> list)
        {
            if (list.Count == 0)
                return null;
            return list[random.Next(0, list.Count)];
        }

        public NineSliceRandom(int seed)
        {
            Seed = seed;
        }

        public override void Draw(Sprite sprite)
        {
            random = new Random(Seed);
            base.Draw(sprite);
        }
    }
}
