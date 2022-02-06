using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public abstract class Component
    {
        public Entity ParentEntity;

        public virtual void Added() { }
        public virtual void Removed() { }

        public virtual void Update() { }

        public void Destroy()
            => ParentEntity.RemoveComponent(this);
    }
}