using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public abstract class Renderer : Component
    {
        public bool Visible = true;
        public virtual void Render() { }
    }
}
