using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System;

namespace Fiourp
{
    public class Map
    {
        public int Width;
        public int Height;
        public List<SolidTile> solidTiles = new List<SolidTile>();
        public MapData Data;
        public Level CurrentLevel;

        public ParticleSystem ForegroundSystem = new ParticleSystem();
        public ParticleSystem MiddlegroundSystem = new ParticleSystem();
        public ParticleSystem BackgroundSystem = new ParticleSystem();


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

        private void DefaultLoad(Level initLevel)
        {
            Engine.CurrentMap = this;
            CurrentLevel = initLevel;
        }

        public void LoadMap(Level initLevel)
        {
            DefaultLoad(initLevel);
            CurrentLevel.Load();
        }

        public void LoadMapNoAutoTile(Level initLevel)
        {
            DefaultLoad(initLevel);
            CurrentLevel.LoadNoAutoTile();
        }

        public void Update()
        {
            for (int i = Data.Entities.Count - 1; i >= 0; i--)
                if (i < Data.Entities.Count && Data.Entities[i].Active)
                    Data.Entities[i].Update();

            BackgroundSystem.Update();
            MiddlegroundSystem.Update();
            ForegroundSystem.Update();

            for (int i = Data.Entities.Count - 1; i >= 0; i--)
                if (i < Data.Entities.Count && Data.Entities[i].Active)
                    Data.Entities[i].LateUpdate();
        }

        public void Render()
        {
            BackgroundSystem.Render();

            for(int i = Data.Decorations.Count - 1; i >= 0; i--)
            {
                if(i < Data.Decorations.Count && Data.Decorations[i].Visible)
                    Data.Decorations[i].Render();
            }

            MiddlegroundSystem.Render();

            int maxLayer = 2;
            List<Entity> loopedEntities = new List<Entity>(Data.Entities);
            for(int l = 0; l <= maxLayer; l++)
            {
                for (int i = loopedEntities.Count - 1; i >= 0; i--)
                {
                    maxLayer = Math.Max(loopedEntities[i].Layer, maxLayer);
                    if (i < loopedEntities.Count && loopedEntities[i].Visible && loopedEntities[i].Tag != Entity.Tags.UI && loopedEntities[i].Tag != Entity.Tags.Decoration && loopedEntities[i].Layer == l)
                    {
                        loopedEntities[i].Render();
                        loopedEntities.RemoveAt(i);
                    }
                }
            }
            

            ForegroundSystem.Render();
        }

        public void UIRender()
        {
            for (int i = Data.UIElements.Count - 1; i >= 0; i--)
                if (i < Data.UIElements.Count && Data.UIElements[i].Visible && !Data.UIElements[i].Overlay)
                    Data.UIElements[i].Render();

            for(int i = Data.Entities.Count - 1; i >= 0; i--)
                if(i < Data.Entities.Count && Data.Entities[i].Visible)
                    Data.Entities[i].UIChildRender();
        }

        public void UIOverlayRender()
        {
            for (int i = Data.UIElements.Count - 1; i >= 0; i--)
                if (i < Data.UIElements.Count && Data.UIElements[i].Visible && Data.UIElements[i].Overlay)
                    Data.UIElements[i].Render();
        }

        public Entity Instantiate(Entity entity)
        {
            Data.Entities.Add(entity);
            entity.Awake();
            if(entity is Platform)
            {
                Data.Platforms.Add((Platform)entity);
                if (entity is Solid)
                    Data.Solids.Add((Solid)entity);
            }
            else if (entity is Actor)
                Data.Actors.Add((Actor)entity);
            else if (entity is Trigger)
                Data.Triggers.Add((Trigger)entity);
            else if (entity is UIElement)
                Data.UIElements.Add((UIElement)entity);
            else if (entity is Tile)
                Data.Decorations.Add((Tile)entity);
            return entity;
        }

        public void Destroy(Entity entity)
        {
            entity.OnDestroy();
            Data.Entities.Remove(entity);

            if (entity is Platform)
            {
                Data.Platforms.Remove((Platform)entity);
                if (entity is Solid)
                    Data.Solids.Remove((Solid)entity);
            }
            else if (entity is Actor)
                Data.Actors.Remove((Actor)entity);
            else if (entity is Trigger)
                Data.Triggers.Remove((Trigger)entity);
            else if (entity is UIElement)
                Data.UIElements.Remove((UIElement)entity);
        }
    }
}