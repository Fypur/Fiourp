using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class Trigger : Entity
    {
        public List<Type> Triggerers;

        private string name;
        private List<Entity> enteredEntities = new List<Entity>();

        public Action<Entity> OnTriggerEnterAction;
        public Action<Entity> OnTriggerStayAction;
        public Action<Entity> OnTriggerExitAction;

        public Trigger(Vector2 position, Vector2 size, List<Type> triggerers, Sprite sprite)
            : base(position, (int)size.X, (int)size.Y, sprite)
        {
            Pos = position;
            Size = size;
            Triggerers = triggerers;
            name = GetType().Name;
        }

        public Trigger(Vector2 position, int width, int height, List<Type> triggerers, Sprite sprite)
            : this(position, new Vector2(width, height), triggerers, sprite)
        { }

        public Trigger(Rectangle bounds, List<Type> triggerers, Sprite sprite)
            : this(bounds.Location.ToVector2(), bounds.Size.ToVector2(), triggerers, sprite)
        { }

        public override void Update()
        {
            base.Update();

            for (int i = Triggerers.Count - 1; i >= 0; i--)
            {
                Engine.CurrentMap.Data.EntitiesByType.TryGetValue(Triggerers[i], out List<Entity> triggers);
                if (triggers == null)
                    continue;

                for (int y = triggers.Count - 1; y >= 0; y--)
                {
                    Entity entity = Engine.CurrentMap.Data.EntitiesByType[Triggerers[i]][y];
                    if (Collider.Collide(entity))
                    {
                        if (enteredEntities.Contains(entity))
                            OnTriggerStay(entity);
                        else
                        {
                            OnTriggerEnter(entity);
                            enteredEntities.Add(entity);
                        }
                    }
                    else if (enteredEntities.Contains(entity))
                    {
                        OnTriggerExit(entity);
                        enteredEntities.Remove(entity);
                    }
                }
            }
        }

        public override void Render() 
        {
            /*if (Debug.DebugMode)
            {
                Drawing.DrawString(name, Pos + Size / 2, Color.Aqua, true);
                Drawing.Draw(new Rectangle(Pos.ToPoint(), Size.ToPoint()), Color.Aqua * 0.2f);
            }*/

            base.Render();
        } 

        public virtual void OnTriggerEnter(Entity entity) { OnTriggerEnterAction?.Invoke(entity); }

        public virtual void OnTriggerStay(Entity entity) { OnTriggerStayAction?.Invoke(entity); }

        public virtual void OnTriggerExit(Entity entity) { OnTriggerExitAction?.Invoke(entity); }
    }
}
