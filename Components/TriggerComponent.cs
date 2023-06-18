using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class TriggerComponent : Renderer
    {
        public Vector2 LocalPosition;
        
        public Vector2 Size { get => Trigger.Size; set => Trigger.Size = value; }
        public List<Type> Triggerers { get => Trigger.Triggerers; set => Trigger.Triggerers = value; }

        public Trigger Trigger;

        public TriggerComponent(Vector2 localPosition, float width, float height, List<Type> triggerers)
        {
            LocalPosition = localPosition;
            Trigger = new Trigger(localPosition, new Vector2(width, height), triggerers, Sprite.None);
        }

        public TriggerComponent(Vector2 localPosition, float radius, List<Type> triggerers)
        {
            LocalPosition = localPosition;
            Trigger = new Trigger(localPosition, new Vector2(radius * 2, radius * 2), triggerers, Sprite.None);
            Trigger.Collider = (Collider)Trigger.AddComponent(new CircleCollider(Vector2.Zero, radius));
        }

        public TriggerComponent(Vector2 localPosition, Collider collider, List<Type> triggerers)
        {
            LocalPosition = localPosition;
            Trigger = new Trigger(localPosition, collider, triggerers, Sprite.None);
        }

        public override void Added()
        {
            base.Added();

            Trigger.OnTriggerEnterAction = OnTriggerEnter;
            Trigger.OnTriggerStayAction = OnTriggerStay;
            Trigger.OnTriggerExitAction = OnTriggerExit;

            Trigger.Pos += ParentEntity.Pos;
        }

        public override void Update()
        {
            Trigger.Pos = ParentEntity.Pos + LocalPosition;
            if(Trigger.Active)
                Trigger.Update();
        }

        public override void Render()
        {
            if(Trigger.Visible)
                Trigger.Render();
        }

        public virtual void OnTriggerEnter(Entity entity) { }
        public virtual void OnTriggerStay(Entity entity) { }
        public virtual void OnTriggerExit(Entity entity) { }
    }
}
