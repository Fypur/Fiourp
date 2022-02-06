using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class MapData
    {
        public Dictionary<Type, List<Entity>> EntitiesByType = new Dictionary<Type, List<Entity>>();
        
        public List<Entity> Entities = new List<Entity>();
        public List<Solid> Solids = new List<Solid>();
        public List<Actor> Actors = new List<Actor>();
        public List<Trigger> Triggers = new List<Trigger>();
        public List<UIElement> UIElements = new List<UIElement>();
    }
}