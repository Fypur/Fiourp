using System;
using System.Collections.Generic;
using System.Linq;

namespace Fiourp
{
    public class MapData
    {
        public List<Entity> Entities = new List<Entity>();

        public Dictionary<Type, List<Entity>> EntitiesByType = new Dictionary<Type, List<Entity>>();
        public List<Rigidbody> Bodies = new List<Rigidbody>();

        public List<T> GetEntities<T>() where T : Entity
        {
            EntitiesByType.TryGetValue(typeof(T), out List<Entity> entities);
            if (entities == null)
                return new List<T>();
            return entities.Cast<T>().ToList();
        }

        public T GetEntity<T>() where T : Entity
        {
            EntitiesByType.TryGetValue(typeof(T), out List<Entity> entities);
            if (entities == null || entities.Count == 0)
                return null;
            return (T)entities[0];
        }
    }
}