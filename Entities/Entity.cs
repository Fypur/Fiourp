using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Fiourp
{
    public class Entity
    {
        public Vector2 Pos;
        public float Rotation { get; set; }

        public bool Active = true;
        public bool Visible = true;

        public int Layer = 0;

        public List<Component> Components = new List<Component>();

        protected List<Entity> Children;
        public Entity Parent = null;

        public Entity(Vector2 position)
        {
            Pos = position;
        }

        public virtual void Awake()
        { }

        public virtual void Update()
        {
            for (int i = Components.Count - 1; i >= 0; i--)
                if (Components.Count > i && Components[i].Active)
                    Components[i].Update();
        }

        public virtual void LateUpdate()
        { }

        public virtual void Render()
        {
            for (int i = Components.Count - 1; i >= 0; i--)
                if (Components[i].Visible)
                    Components[i].Render();
        }

        public virtual void OnDestroy()
        {
            for (int i = Components.Count - 1; i >= 0; i--)
                Components[i].Removed();
        }

        public virtual bool CollidingConditions(Collider other)
            => true;

        public virtual void Move(Vector2 moveAmount)
            => Pos += moveAmount;

        public Component AddComponent(Component component)
        {
            component.ParentEntity = this;
            component.Added();
            Components.Add(component);

            return component;
        }

        public void RemoveAllComponents<T>() where T : Component
        {
            for (int i = Components.Count - 1; i >= 0; i--)
                if (Components[i] is T)
                {
                    RemoveComponent(Components[i]);
                }
        }

        public void RemoveComponent(Component component)
        {
            Components.Remove(component);
            component?.Removed();
        }

        public bool HasComponent<T>() where T : Component
        {
            foreach (Component c in Components)
                if (c is T t)
                    return true;

            return false;
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (Component c in Components)
                if (c is T t)
                    return (T)c;

            return null;
        }

        public List<T> GetAllComponents<T>() where T : Component
        {
            List<T> result = new();
            foreach (Component c in Components)
            {
                if (c is T t)
                    result.Add(t);
            }

            return result;
        }

        public bool TryGetComponent<T>(out T component) where T : Component
        {
            foreach (Component c in Components)
                if (c is T t)
                {
                    component = t;
                    return true;
                }

            component = null;
            return false;
        }

        public void AddChild(Entity child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        public void RemoveChild(Entity child)
        {
            Children.Remove(child);
            child.Parent = null;
        }

        public void SelfDestroy()
        {
            Engine.CurrentMap.Destroy(this);
        }
    }
}
