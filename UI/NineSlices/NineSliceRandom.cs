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

        protected override Sprite TopRightTile { get => GetRandomTile(TopRight); }
        protected override Sprite TopLeftTile { get => GetRandomTile(TopLeft); }
        protected override Sprite BottomRightTile { get => GetRandomTile(BottomRight); }
        protected override Sprite BottomLeftTile { get => GetRandomTile(BottomLeft); }
        protected override Sprite TopTile { get => GetRandomTile(Top); }
        protected override Sprite LeftTile { get => GetRandomTile(Left); }
        protected override Sprite RightTile { get => GetRandomTile(Right); }
        protected override Sprite BottomTile { get => GetRandomTile(Bottom); }
        protected override Sprite FillTile { get => GetRandomTile(Fill); }

        private Sprite GetRandomTile(List<Texture2D> list)
        {
            if (list.Count == 0)
                return null;
            return new Sprite(list[random.Next(0, list.Count)]);
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

        public override void Update()
        {
            
        }
    }
}
