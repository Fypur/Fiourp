using System;
using System.Collections.Generic;

namespace Fiourp
{
    public class Map
    {
        public MapData Data;
        public int MinLayer { get; private set; } = -3;
        public int MaxLayer { get; private set; } = 2;

        public ParticleSystem ForegroundSystem = new ParticleSystem();
        public ParticleSystem MiddlegroundSystem = new ParticleSystem();
        public ParticleSystem BackgroundSystem = new ParticleSystem();

        public Map()
        {
            Data = new MapData();
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
            List<Entity> loopedEntities = new List<Entity>(Data.Entities);

            for (int l = -MinLayer; l <= MaxLayer; l++)
            {
                for (int i = loopedEntities.Count - 1; i >= 0; i--)
                {
                    if (i >= loopedEntities.Count)
                        break;

                    MaxLayer = Math.Max(loopedEntities[i].Layer, MaxLayer);
                    MinLayer = Math.Min(loopedEntities[i].Layer, MinLayer); //technically this is updated a frame too late, but I don't wanna loop twice

                    if (loopedEntities[i].Visible && loopedEntities[i].Layer == l)
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

        public Entity Instantiate(Entity entity)
        {
            Data.Entities.Add(entity);

            Type type = entity.GetType();
            if (!Engine.CurrentMap.Data.EntitiesByType.ContainsKey(type))
                Engine.CurrentMap.Data.EntitiesByType.Add(type, new List<Entity>() { entity });
            else
                Engine.CurrentMap.Data.EntitiesByType[type].Add(entity);

            entity.Awake();
            return entity;
        }

        public void Destroy(Entity entity)
        {
            for (int i = entity.Components.Count - 1; i >= 0; i--)
                if (i < entity.Components.Count)
                    entity.Components[i].Destroy();

            Data.Entities.Remove(entity);

            Engine.CurrentMap.Data.EntitiesByType[entity.GetType()].Remove(entity);
            entity.OnDestroy();
        }
    }
}