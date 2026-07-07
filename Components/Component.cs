namespace Fiourp
{
    public abstract class Component
    {
        public bool Active = true;
        public bool Visible = true;

        public Entity ParentEntity;

        public virtual void Added() { }
        public virtual void Removed() { }

        public virtual void Update() { }
        public virtual void Render() { }

        public void SelfDestroy()
            => ParentEntity.RemoveComponent(this);
    }
}