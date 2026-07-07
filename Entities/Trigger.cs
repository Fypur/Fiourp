using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Fiourp
{
    public class Trigger : Entity
    {
        public static List<Trigger> InstanciatedTriggers = new List<Trigger>();

        public Collider Collider;
        public List<Type> Triggerers;

        private List<Kinematic> enteredEntities = new List<Kinematic>();

        public Trigger(Vector2 position, Collider collider, List<Type> triggerers) : base(position)
        {
            foreach (Type t in triggerers)
            {
                if (!typeof(Kinematic).IsAssignableFrom(t))
                    throw new Exception($"The Trigger class doesn't function with entities that don't inherit from Kinematic such as {t.Name}.");
            }

            Pos = position;
            Triggerers = triggerers;

            Collider = collider;
            Collider.DebugColor = Color.White;
            AddComponent(Collider);
        }

        public override void Awake()
        {
            base.Awake();

            InstanciatedTriggers.Add(this);
        }

        public override void Update()
        {
            base.Update();

            for (int i = Triggerers.Count - 1; i >= 0; i--)
            {
                Engine.CurrentMap.Data.EntitiesByType.TryGetValue(Triggerers[i], out DeferredList<Entity> triggers);

                if (triggers == null)
                    continue;

                foreach (Entity entity in triggers.Items)
                {
                    Kinematic kinematic = (Kinematic)entity;

                    if (Collider.Collide(kinematic.Collider))
                    {
                        if (enteredEntities.Contains(kinematic))
                            OnTriggerStay(kinematic);
                        else
                            OnTriggerEnter(kinematic);
                    }
                    else if (enteredEntities.Contains(kinematic))
                        OnTriggerExit(kinematic);
                }
            }
        }

        public bool Contains(Kinematic entity)
            => enteredEntities.Contains(entity);

        public virtual void OnTriggerEnter(Kinematic entity)
            => enteredEntities.Add(entity);

        public virtual void OnTriggerStay(Kinematic entity)
        { }

        public virtual void OnTriggerExit(Kinematic entity)
            => enteredEntities.Remove(entity);

        public override void OnDestroy()
        {
            InstanciatedTriggers.Remove(this);
            for (int i = enteredEntities.Count - 1; i >= 0; i--)
                OnTriggerExit(enteredEntities[i]);

            base.OnDestroy();
        }
    }
}
