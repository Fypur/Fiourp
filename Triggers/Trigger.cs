using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Fiourp
{
    public class Trigger : Entity
    {
        public List<Type> Triggerers;

        private List<Entity> enteredEntities = new List<Entity>();

        public Trigger(Vector2 position, Vector2 size, List<Type> triggerers, Sprite sprite)
            : base(position, (int)size.X, (int)size.Y, sprite)
        {
            Pos = position;
            Size = size;
            Triggerers = triggerers;

            Collider = new AABBCollider(Vector2.Zero, (int)size.X, (int)size.Y);
            AddComponent(Collider);

            Collider.DebugColor = Color.White;
        }

        public Trigger(Vector2 position, int width, int height, List<Type> triggerers, Sprite sprite)
            : this(position, new Vector2(width, height), triggerers, sprite)
        { }

        public Trigger(Rectangle bounds, List<Type> triggerers, Sprite sprite)
            : this(bounds.Location.ToVector2(), bounds.Size.ToVector2(), triggerers, sprite)
        { }

        public Trigger(Vector2 position, AABBCollider collider, List<Type> triggerers, Sprite sprite)
            : this(position, 1, 1, triggerers, sprite)
        {
            RemoveComponent(Collider);
            Collider = collider;
            AddComponent(Collider);

            Width = collider.Width;
            Height = collider.Height;
        }

        public override void Update()
        {
            base.Update();

            for (int i = Triggerers.Count - 1; i >= 0; i--)
            {
                Engine.CurrentMap.Data.EntitiesByType.TryGetValue(Triggerers[i], out List<Entity> triggers);

                if (triggers == null)
                    continue;

                for (int y = triggers.Count - 1; y >= 0; y--)
                {
                    Entity entity = Engine.CurrentMap.Data.EntitiesByType[Triggerers[i]][y];
                    if (Collider.Collide(entity))
                    {
                        if (enteredEntities.Contains(entity))
                            OnTriggerStay(entity);
                        else
                            OnTriggerEnter(entity);
                    }
                    else if (enteredEntities.Contains(entity))
                        OnTriggerExit(entity);
                }
            }
        }

        public bool Contains(Entity entity)
            => enteredEntities.Contains(entity);

        public virtual void OnTriggerEnter(Entity entity)
            => enteredEntities.Add(entity);

        public virtual void OnTriggerStay(Entity entity)
        { }

        public virtual void OnTriggerExit(Entity entity)
            => enteredEntities.Remove(entity);

        public override void OnDestroy()
        {
            for (int i = enteredEntities.Count - 1; i >= 0; i--)
                OnTriggerExit(enteredEntities[i]);

            base.OnDestroy();
        }
    }
}
