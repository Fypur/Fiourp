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

        private DeferredList<Kinematic> deferredKinematics = new();
        private DeferredList<Kinematic> deferredNonActorKinematics = new();
        private DeferredList<Solid> deferredSolids = new();
        private DeferredList<Actor> deferredActors = new();

        public IReadOnlyList<Kinematic> Kinematics => deferredKinematics.Items;
        public IReadOnlyList<Kinematic> NonActorKinematics => deferredNonActorKinematics.Items;
        public IReadOnlyList<Solid> Solids => deferredSolids.Items;
        public IReadOnlyList<Actor> Actors => deferredActors.Items;

        public Map()
        {
            Data = new MapData();
        }

        public virtual void Update()
        {
            foreach (Entity entity in Data.Entities.Items)
                if (entity.Active)
                    entity.Update();

            BackgroundSystem.Update();
            MiddlegroundSystem.Update();
            ForegroundSystem.Update();
        }

        public virtual void LateUpdate()
        {
            foreach (Entity entity in Data.Entities.Items)
                entity.LateUpdate();

            deferredKinematics.ProcessChanges();
            deferredNonActorKinematics.ProcessChanges();
            deferredSolids.ProcessChanges();
            deferredActors.ProcessChanges();

            foreach (DeferredList<Entity> deferredList in Data.EntitiesByType.Values)
                deferredList.ProcessChanges();

            Data.Entities.ProcessChanges((entity) =>
                {
                    entity.ParentMap = this;
                    entity.Awake();
                },
                (entity) =>
                {
                    entity.OnDestroy();
                    entity.ParentMap = null;
                }
                );
        }

        public virtual void Render()
        {
            List<Entity> loopedEntities = new List<Entity>(Data.Entities.Items);

            for (int l = MinLayer; l <= MaxLayer; l++)
            {
                for (int i = loopedEntities.Count - 1; i >= 0; i--)
                {
                    if (i >= loopedEntities.Count)
                        break;

                    MaxLayer = Math.Max(loopedEntities[i].Layer, MaxLayer);
                    MinLayer = Math.Min(loopedEntities[i].Layer, MinLayer); //technically the item with a new minimum layer will be renderered a frame too late, but I don't wanna loop twice to check for the actual minLayer at every render

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
                Engine.CurrentMap.Data.EntitiesByType.Add(type, new DeferredList<Entity>());

            Engine.CurrentMap.Data.EntitiesByType[type].Add(entity);

            if (entity is Kinematic kinematic)
            {
                if (entity is Actor actor)
                    deferredActors.Add(actor);
                else
                    deferredNonActorKinematics.Add(kinematic);

                if (entity is Solid solid)
                    deferredSolids.Add(solid);

                deferredKinematics.Add(kinematic);
            }

            return entity;
        }

        public virtual void Destroy(Entity entity)
        {
            Data.Entities.Remove(entity);

            Engine.CurrentMap.Data.EntitiesByType[entity.GetType()].Remove(entity);

            if (entity is Kinematic kinematic)
            {
                if (entity is Actor actor)
                    deferredActors.Remove(actor);
                else
                    deferredNonActorKinematics.Remove(kinematic);

                if (entity is Solid solid)
                    deferredSolids.Remove(solid);

                deferredKinematics.Remove(kinematic);
            }
        }
    }
}