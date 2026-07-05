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

        public List<Kinematic> Kinematics = new();
        public List<Kinematic> NonActorKinematics = new();
        public List<Solid> Solids = new();
        public List<Actor> Actors = new();

        public Map()
        {
            Data = new MapData();
        }

        public virtual void Update()
        {
            for (int i = Data.Entities.Count - 1; i >= 0; i--)
                if (i < Data.Entities.Count && Data.Entities[i].Active)
                    Data.Entities[i].Update();

            BackgroundSystem.Update();
            MiddlegroundSystem.Update();
            ForegroundSystem.Update();
        }

        public virtual void LateUpdate()
        {
            for (int i = Data.Entities.Count - 1; i >= 0; i--)
                if (i < Data.Entities.Count && Data.Entities[i].Active)
                    Data.Entities[i].LateUpdate();
        }

        public virtual void Render()
        {
            List<Entity> loopedEntities = new List<Entity>(Data.Entities);

            for (int l = MinLayer; l <= MaxLayer; l++)
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

        public virtual Entity Instantiate(Entity entity)
        {
            Data.Entities.Add(entity);

            Type type = entity.GetType();
            if (!Engine.CurrentMap.Data.EntitiesByType.ContainsKey(type))
                Engine.CurrentMap.Data.EntitiesByType.Add(type, new List<Entity>() { entity });
            else
                Engine.CurrentMap.Data.EntitiesByType[type].Add(entity);

            if (entity is Kinematic kinematic)
            {
                if (entity is Actor actor)
                    Actors.Add(actor);
                else
                    NonActorKinematics.Add(kinematic);

                if (entity is Solid solid)
                    Solids.Add(solid);

                Kinematics.Add(kinematic);
            }

            entity.ParentMap = this;

            entity.Awake();
            return entity;
        }

        public virtual void Destroy(Entity entity)
        {
            for (int i = entity.Components.Count - 1; i >= 0; i--)
                if (i < entity.Components.Count)
                    entity.Components[i].Destroy();

            Data.Entities.Remove(entity);

            Engine.CurrentMap.Data.EntitiesByType[entity.GetType()].Remove(entity);

            if (entity is Kinematic kinematic)
            {
                if (entity is Actor actor)
                    Actors.Remove(actor);
                else
                    NonActorKinematics.Remove(kinematic);

                if (entity is Solid solid)
                    Solids.Remove(solid);

                Kinematics.Remove(kinematic);
            }

            entity.OnDestroy();

            entity.ParentMap = null;
        }
    }
}