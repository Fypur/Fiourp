using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class Entity
    {
        public Vector2 Pos;
        public int Width;
        public int Height;

        public bool Active = true;
        public bool Visible = true;

        public Tags Tag;
        public enum Tags { Unknown, Actor, Solid, Trigger, UI, Decoration }
        public int Layer = 0;

        public virtual Vector2 ExactPos { get => Pos; set => Pos = value; }
        public Vector2 MiddleExactPos => ExactPos + HalfSize;
        public Vector2 MiddlePos { get => Pos + HalfSize; set { Pos = value - HalfSize; } }
        public Vector2 Size { get => new Vector2(Width, Height); set { Width = (int)value.X; Height = (int)value.Y; } }
        public Vector2 HalfSize { get => new Vector2(Width / 2, Height / 2); }
        public Rectangle Bounds { get => new Rectangle(Pos.ToPoint(), Size.ToPoint()); set { Pos = value.Location.ToVector2(); Size = value.Size.ToVector2(); } }

        public Collider Collider;
        public Sprite Sprite;

        public List<Component> Components = new List<Component>();
        public List<Renderer> Renderers = new List<Renderer>();

        public List<Entity> Children = new List<Entity>();
        public Vector2 PreviousPos;

        public Entity(Vector2 position, int width, int height, Sprite sprite)
        {
            ExactPos = position;
            Width = width;
            Height = height;

            Type t = GetType();
            if (!Engine.CurrentMap.Data.EntitiesByType.ContainsKey(t))
                Engine.CurrentMap.Data.EntitiesByType.Add(t, new List<Entity>() { this });
            else
                Engine.CurrentMap.Data.EntitiesByType[t].Add(this);

            Tag = this switch
            {
                Actor => Tags.Actor,
                Solid => Tags.Solid,
                Trigger => Tags.Trigger,
                UIElement => Tags.UI,
                Tile => Tags.Decoration,
                _ => Tags.Unknown
            };

            Collider = new BoxCollider(Vector2.Zero, width, height);
            AddComponent(Collider);

            if(sprite != null)
            {
                Sprite = sprite;
                AddComponent(Sprite);
            }
        }

        public Entity(Vector2 position)
        {
            Pos = position;
            Width = 0;
            Height = 0;

            Tag = this switch
            {
                Actor => Tags.Actor,
                Solid => Tags.Solid,
                Trigger => Tags.Trigger,
                UIElement => Tags.UI,
                Tile => Tags.Decoration,
                _ => Tags.Unknown
            };
        }

        /// <summary>
        /// Called after constructors when the Entity is Instantiated
        /// </summary>
        public virtual void Awake()
        {
            PreviousPos = Pos;
        }

        public virtual void Update()
        {
            for (int i = Components.Count - 1; i >= 0; i--)
                if(Components[i].Active)
                    Components[i].Update();

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                if (i >= Children.Count)
                    return;

                Children[i].Pos += Pos - PreviousPos;
                if(Pos - PreviousPos != Vector2.Zero)
                { }
                if (Children[i].Active)
                    Children[i].Update();
            }

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                if (i >= Children.Count)
                    return;

                if (Children[i].Active)
                    Children[i].LateUpdate();
            }

            PreviousPos = Pos;
        }

        public virtual void LateUpdate()
        {

        }

        public void UpdateChildrenPos()
        {
            foreach(Entity child in Children)
            {
                child.Pos += Pos - PreviousPos;
            }

            PreviousPos = Pos;
        }

        public virtual void Render()
        {
            for (int i = Renderers.Count - 1; i >= 0; i--)
                if(Renderers[i].Visible)
                    Renderers[i].Render();

            for (int i = Children.Count - 1; i >= 0; i--)
                if(Children[i].Visible && Children[i].Tag != Tags.UI)
                    Children[i].Render();

            if (Debug.DebugMode)
                Collider?.Render();
        }

        public void UIChildRender()
        {
            for (int i = Children.Count - 1; i >= 0; i--)
                if (Children[i].Active && Children[i].Tag == Tags.UI)
                {
                    Children[i].Render();
                    Children[i].UIChildRender();
                }
        }

        public virtual void OnDestroy()
        {
            Engine.CurrentMap.Data.EntitiesByType[GetType()].Remove(this);
        }

        public virtual bool CollidingConditions(Collider other)
            => true;

        public Component AddComponent(Component component)
        {
            component.ParentEntity = this;
            component.Added();
            Components.Add(component);

            if (component is Renderer renderer)
                Renderers.Add(renderer);

            return component;
        }

        public void RemoveComponent<T>() where T : Component
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

            if (component is Renderer renderer)
                Renderers.Remove(renderer);
        }

        public bool HasComponent<T>() where T : Component
        {
            foreach (Component c in Components)
                if (c is T t)
                    return true;

            return false;
        }

        public bool HasComponent<T>(out T component) where T : Component
        {
            foreach(Component c in Components)
            {
                if(c is T t)
                {
                    component = t;
                    return true;
                }
            }

            component = null;
            return false;
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (Component c in Components)
                if (c is T t)
                    return (T)c;

            return null;
        }

        public List<T> GetComponents<T>() where T : Component
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
            foreach(Component c in Components)
                if(c is T t)
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
            child.Awake();
            return child;
        }

        public void AddChildren(List<Entity> children)
        {
            foreach(Entity child in children)
                AddChild(child);
        }

        public void RemoveChild(Entity child)
        {
            Children.Remove(child);
            child.OnDestroy();
        }

        public bool HasChild<T>(out T component) where T : Entity
        {
            foreach (Entity c in Children)
            {
                if (c is T t)
                {
                    component = t;
                    return true;
                }
            }

            component = null;
            return false;
        }

        public void SelfDestroy()
            => Engine.CurrentMap.Destroy(this);
    }
}
