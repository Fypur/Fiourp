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
            CurrentLevel.LoadAutoTile();
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
        }

        public void LateUpdate()
        {
            for (int i = Data.Entities.Count - 1; i >= 0; i--)
                if (i < Data.Entities.Count && Data.Entities[i].Active)
                    Data.Entities[i].LateUpdate();
        }

        public void Render()
        {
            int maxLayer = 2;
            List<Entity> loopedEntities = new List<Entity>(Data.Entities);
            for(int l = -3; l <= maxLayer; l++)
            {
                for (int i = loopedEntities.Count - 1; i >= 0; i--)
                {
                    maxLayer = Math.Max(loopedEntities[i].Layer, maxLayer);
                    if (i < loopedEntities.Count && loopedEntities[i].Visible && loopedEntities[i].Tag != Entity.Tags.UI && loopedEntities[i].Layer == l)
                    {
                        loopedEntities[i].Render();
                        loopedEntities.RemoveAt(i);
                    }
                }

                if (l == 0)
                    BackgroundSystem.Render();


                if (l == 1)
                    MiddlegroundSystem.Render();
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
            if(entity is Platform p)
            {
                Data.Platforms.Add(p);
                if (entity is Solid s)
                    Data.Solids.Add(s);
            }
            else if (entity is Actor a)
                Data.Actors.Add(a);
            else if (entity is Trigger t)
                Data.Triggers.Add(t);
            else if (entity is UIElement u)
                Data.UIElements.Add(u);
            else if (entity is Decoration d)
                Data.Decorations.Add(d);

            Type type = entity.GetType();
            if (!Engine.CurrentMap.Data.EntitiesByType.ContainsKey(type))
                Engine.CurrentMap.Data.EntitiesByType.Add(type, new List<Entity>() { entity });
            else
                Engine.CurrentMap.Data.EntitiesByType[type].Add(entity);

            return entity;
        }

        public void Destroy(Entity entity)
        {
            for(int i = entity.Components.Count - 1; i >= 0; i--)
                if(i < entity.Components.Count)
                    entity.Components[i].Destroy();

            entity.OnDestroy();

            Data.Entities.Remove(entity);

            if (entity is Platform p)
            {
                Data.Platforms.Remove(p);
                if (entity is Solid s)
                {
                    Data.Solids.Remove(s);
                    if (Data.CameraSolids.Contains(s))
                        Data.CameraSolids.Remove(s);
                }
            }
            else if (entity is Actor a)
                Data.Actors.Remove(a);
            else if (entity is Trigger t)
                Data.Triggers.Remove(t);
            else if (entity is UIElement u)
                Data.UIElements.Remove(u);
            else if (entity is Decoration d)
                Data.Decorations.Remove(d);

            Engine.CurrentMap.Data.EntitiesByType[entity.GetType()].Remove(entity);
        }
    }
}