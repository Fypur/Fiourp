using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public abstract class Trigger : Entity
    {
        public List<Type> Triggerers;

        private string name;
        private List<Entity> enteredEntities = new List<Entity>();

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

        public Trigger(Rectangle triggerRect, List<Type> triggerers, Sprite sprite)
            : this(triggerRect.Location.ToVector2(), triggerRect.Size.ToVector2(), triggerers, sprite)
        { }

        public override void Update()
        {
            base.Update();

            for (int i = Triggerers.Count - 1; i >= 0; i--)
                foreach (Entity entity in Engine.CurrentMap.Data.EntitiesByType[Triggerers[i]])
                {
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

        public override void Render() 
        {
            /*if (Debug.DebugMode)
            {
                Drawing.DrawString(name, Pos + Size / 2, Color.Aqua, true);
                Drawing.Draw(new Rectangle(Pos.ToPoint(), Size.ToPoint()), Color.Aqua * 0.2f);
            }*/

            base.Render();
        } 

        public virtual void OnTriggerEnter(Entity entity) { }

        public virtual void OnTriggerStay(Entity entity) { }

        public virtual void OnTriggerExit(Entity entity) { }
    }
}
