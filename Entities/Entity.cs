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

        private List<Component> components = new();

        protected List<Entity> Children = new();
        public Entity Parent = null;

        public Map ParentMap;

        public Entity(Vector2 position)
        {
            Pos = position;
        }

        public virtual void Awake()
        {
        }

        public virtual void Update()
        {
            for (int i = components.Count - 1; i >= 0; i--)
                if (i < components.Count && components[i].Active)
                    components[i].Update();
        }

        public virtual void LateUpdate()
        { }

        public virtual void Render()
        {
            for (int i = components.Count - 1; i >= 0; i--)
                if (i < components.Count && components[i].Visible)
                    components[i].Render();
        }

        public virtual void OnDestroy()
        {
            for (int i = components.Count - 1; i >= 0; i--)
                if (i < components.Count)
                    components[i].Removed();

            for (int i = components.Count - 1; i >= 0; i--)
                if (i < components.Count)
                    components[i].SelfDestroy();
        }

        public virtual bool CollidingConditions(Collider other)
            => true;

        /// <summary>
        /// Returns true amount moved
        /// </summary>
        public virtual Vector2 Move(Vector2 moveAmount)
        {
            Pos += moveAmount;
            return moveAmount;
        }

        public Component AddComponent(Component component)
        {
            component.ParentEntity = this;
            components.Add(component);

            component.Added();
            return component;
        }

        public void RemoveComponent(Component component)
        {
            component.Removed();
            component.ParentEntity = null;
            components.Remove(component);
        }

        public void RemoveAllComponentsOfType<T>() where T : Component
        {
            for (int i = components.Count - 1; i >= 0; i--)
                if (components[i] is T)
                    RemoveComponent(components[i]);
        }

        public bool HasComponent<T>() where T : Component
        {
            foreach (Component component in components)
                if (component is T t)
                    return true;

            return false;
        }

        public bool HasComponent(Component component)
        {
            foreach (Component c in components)
                if (c == component)
                    return true;

            return false;
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (Component c in components)
                if (c is T t)
                    return (T)c;

            return null;
        }

        public List<T> GetAllComponents<T>() where T : Component
        {
            List<T> result = new();
            foreach (Component c in components)
            {
                if (c is T t)
                    result.Add(t);
            }

            return result;
        }

        public bool TryGetComponent<T>(out T component) where T : Component
        {
            foreach (Component c in components)
                if (c is T t)
                {
                    component = t;
                    return true;
                }

            component = null;
            return false;
        }

        public Entity AddChild(Entity child)
        {
            Children.Add(child);
            child.Parent = this;
            return child;
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
