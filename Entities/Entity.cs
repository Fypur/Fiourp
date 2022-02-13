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

        public Tags Tag;
        public enum Tags { Unknown, Actor, Solid, Trigger, UI }

        public virtual Vector2 ExactPos => Pos;
        public Vector2 Size { get => new Vector2(Width, Height); set { Width = (int)value.X; Height = (int)value.Y; } }
        public Vector2 HalfSize { get => new Vector2(Width / 2, Height / 2); }
        public Rectangle Rect { get => new Rectangle(Pos.ToPoint(), Size.ToPoint()); set { Pos = value.Location.ToVector2(); Size = value.Size.ToVector2(); } }

        public Collider Collider;
        public Sprite Sprite;

        public List<Component> components = new List<Component>();
        public List<Renderer> renderers = new List<Renderer>();

        public List<Entity> Children = new List<Entity>();
        private List<Vector2> childrenPositionOffset = new List<Vector2>();

        public Entity(Vector2 position, int width, int height, Sprite sprite)
        {
            Pos = position;
            Width = width;
            Height = height;

            #region Entities By Type
            Type t = GetType();
            if (!Engine.CurrentMap.Data.EntitiesByType.ContainsKey(t))
                Engine.CurrentMap.Data.EntitiesByType.Add(t, new List<Entity>() { this });
            else
                Engine.CurrentMap.Data.EntitiesByType[t].Add(this);
            #endregion

            #region Tag set

            Tag = this switch
            {
                Actor => Tags.Actor,
                Solid => Tags.Solid,
                Trigger => Tags.Trigger,
                UIElement => Tags.UI,
                _ => Tags.Unknown
            };

            #endregion

            Collider = new BoxCollider(Vector2.Zero, width, height);
            AddComponent(Collider);

            if(sprite != null)
            {
                Sprite = sprite;
                AddComponent(Sprite);
            }
        }

        public virtual void Update()
        {
            for (int i = components.Count - 1; i >= 0; i--)
                components[i].Update();

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                Children[i].Pos = Pos + childrenPositionOffset[i];
                Children[i].Update();
            }
        }

        public virtual void Render()
        {
            for (int i = renderers.Count - 1; i >= 0; i--)
                renderers[i].Render();

            for (int i = Children.Count - 1; i >= 0; i--)
                Children[i].Render();

            if (Debug.DebugMode)
                Collider?.Render();
        }

        public Component AddComponent(Component component)
        {
            component.ParentEntity = this;
            component.Added();
            components.Add(component);

            if (component is Renderer renderer)
                renderers.Add(renderer);

            return component;
        }

        public void RemoveComponent(Component component)
        {
            components.Remove(component);

            if (component is Renderer renderer)
                renderers.Remove(renderer);
        }

        public bool GetComponent<T>(out T component)
        {
            foreach(Component c in components)
                if(c is T t)
                {
                    component = t;
                    return true;
                }

            component = default(T);
            return false;
        }

        public void AddChild(Entity child)
        {
            Children.Add(child);
            childrenPositionOffset.Add(child.Pos - Pos);
        }

        public void RemoveChild(Entity child)
        {
            Children.Remove(child);
            childrenPositionOffset.Remove(child.Pos - Pos);
        }
    }
}
