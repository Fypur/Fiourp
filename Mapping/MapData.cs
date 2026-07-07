using System;
using System.Collections.Generic;
using System.Linq;

namespace Fiourp
{
    public class MapData
    {
        public DeferredList<Entity> Entities = new();

        public Dictionary<Type, DeferredList<Entity>> EntitiesByType = new Dictionary<Type, DeferredList<Entity>>();
        public List<Rigidbody> Bodies = new List<Rigidbody>();

        public List<T> GetEntities<T>() where T : Entity
        {
            EntitiesByType.TryGetValue(typeof(T), out DeferredList<Entity> entities);
            if (entities == null)
                return new List<T>();
            return entities.Items.Cast<T>().ToList();
        }

        public T GetEntity<T>() where T : Entity
        {
            EntitiesByType.TryGetValue(typeof(T), out DeferredList<Entity> entities);
            if (entities == null || entities.Items.Count == 0)
                return null;
            return (T)entities.Items[0];
        }
    }
}