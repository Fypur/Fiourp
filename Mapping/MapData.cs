using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fiourp
{
    public class MapData
    {
        public Dictionary<Type, List<Entity>> EntitiesByType = new Dictionary<Type, List<Entity>>();
        
        public List<Entity> Entities = new List<Entity>();
        public List<Platform> Platforms = new List<Platform>();
        public List<Solid> Solids = new List<Solid>();
        public List<Solid> CameraSolids = new List<Solid>();
        public List<Actor> Actors = new List<Actor>();
        public List<Trigger> Triggers = new List<Trigger>();
        public List<UIElement> UIElements = new List<UIElement>();
        public List<Decoration> Decorations = new List<Decoration>();

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