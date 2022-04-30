using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class TriggerComponent : Renderer
    {
        public Vector2 LocalPosition;
        
        public Vector2 Size { get => trigger.Size; set => trigger.Size = value; }
        public List<Type> Triggerers { get => trigger.Triggerers; set => trigger.Triggerers = value; }

        private Trigger trigger;

        public TriggerComponent(Vector2 localPosition, float width, float height, List<Type> triggerers)
        {
            LocalPosition = localPosition;
            trigger = new Trigger(localPosition, new Vector2(width, height), triggerers, Sprite.None);
            trigger.OnTriggerEnterAction = OnTriggerEnter;
            trigger.OnTriggerStayAction = OnTriggerStay;
            trigger.OnTriggerExitAction = OnTriggerExit;
        }

        public TriggerComponent(Vector2 localPosition, float radius, List<Type> triggerers)
        {
            LocalPosition = localPosition;
            trigger = new Trigger(localPosition, new Vector2(radius * 2, radius * 2), triggerers, Sprite.None);
            trigger.Collider = (Collider)trigger.AddComponent(new CircleCollider(Vector2.Zero, radius));
            trigger.OnTriggerEnterAction = OnTriggerEnter;
            trigger.OnTriggerStayAction = OnTriggerStay;
            trigger.OnTriggerExitAction = OnTriggerExit;
        }

        public override void Added()
        {
            trigger.Pos += ParentEntity.Pos;
        }

        public override void Update()
        {
            trigger.Pos = ParentEntity.Pos + LocalPosition;
            trigger.Update();
        }

        public override void Render()
        {
            trigger.Render();
        }

        public virtual void OnTriggerEnter(Entity entity) { }
        public virtual void OnTriggerStay(Entity entity) { }
        public virtual void OnTriggerExit(Entity entity) { }
    }
}
