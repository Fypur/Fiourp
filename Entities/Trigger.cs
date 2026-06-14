using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Fiourp
{
    public class Trigger : Entity
    {
        public Collider Collider;
        public List<Type> Triggerers;

        private List<Entity> enteredEntities = new List<Entity>();

        public Trigger(Vector2 position, Collider collider, List<Type> triggerers) : base(position)
        {
            Pos = position;
            Triggerers = triggerers;

            Collider = collider;
            Collider.DebugColor = Color.White;
            AddComponent(Collider);
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
