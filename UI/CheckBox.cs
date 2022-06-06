using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class CheckBox : Button
    {
        private bool activated;
        public CheckBox(Vector2 position, int width, int height, bool centered, bool active) : base(position, width, height, centered, active ? new Sprite(Color.White) : new Sprite(Color.Black))
        { activated = active; }

        public override void OnClick()
        {
            activated = !activated;
            Sprite.Color = activated ? Color.White : Color.Black;
        }
    }
}
