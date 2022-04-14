using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Fiourp
{
    public class Map
    {
        public int Width;
        public int Height;
        public List<SolidTile> solidTiles = new List<SolidTile>();
        public MapData Data;
        public Level CurrentLevel;

        public static Map CurrentMap { get => Engine.CurrentMap; }

        /// <summary>
        /// Map constructor
        /// </summary>
        /// <param name="position"></param>
        /// <param name="mapWidth">Max number of horizontal tiles</param>
        /// <param name="mapHeight">Max number of vertical tiles</param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <param name="mapOrganisation">0 for nothing placed, a 1 for a tile placed, lenght of the array must be equal to mapWidth * mapHeight</param>
        public Map(Vector2 position)
        {
            Data = new MapData();
        }

        public void LoadMap(Level initLevel)
        {
            Engine.CurrentMap = this;
            CurrentLevel = initLevel;
            CurrentLevel.Load();
        }

        public void Update()
        {
            for (int i = Data.Entities.Count - 1; i >= 0; i--)
                if (i < Data.Entities.Count && Data.Entities[i].Active)
                    Data.Entities[i].Update();
        }

        public void Render()
        {
            for (int i = Data.Entities.Count - 1; i >= 0; i--)
            {
                if(i < Data.Entities.Count && Data.Entities[i].Active && Data.Entities[i].Tag != Entity.Tags.UI)
                    Data.Entities[i].Render();
            }

            for (int i = Data.Triggers.Count - 1; i >= 0; i--)
                if (i < Data.Triggers.Count && Data.Triggers[i].Active)
                    Data.Triggers[i].Render();
        }

        public void UIRender()
        {
            for (int i = Data.UIElements.Count - 1; i >= 0; i--)
                if (i < Data.UIElements.Count && Data.UIElements[i].Active)
                    Data.UIElements[i].Render();

            for(int i = Data.Entities.Count - 1; i >= 0; i--)
                if(i < Data.Entities.Count && Data.Entities[i].Active)
                    Data.Entities[i].UIChildRender();
        }

        public Entity Instantiate(Entity entity)
        {
            Data.Entities.Add(entity);

            if (entity is Solid)
                Data.Solids.Add((Solid)entity);
            else if (entity is Actor)
                Data.Actors.Add((Actor)entity);
            else if (entity is Trigger)
                Data.Triggers.Add((Trigger)entity);
            else if (entity is UIElement)
                Data.UIElements.Add((UIElement)entity);
            return entity;
        }

        public void Destroy(Entity entity)
        {
            entity.OnDestroy();

            Data.Entities.Remove(entity);

            if (entity is Solid)
                Data.Solids.Remove((Solid)entity);
            else if (entity is Actor)
                Data.Actors.Remove((Actor)entity);
            else if (entity is Trigger)
                Data.Triggers.Remove((Trigger)entity);
            else if (entity is UIElement)
                Data.UIElements.Remove((UIElement)entity);
        }
    }
}