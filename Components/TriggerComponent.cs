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
        public Action<Entity> OnTriggerEnter { get => trigger.OnTriggerEnterAction; set => trigger.OnTriggerEnterAction = value; }
        public Action<Entity> OnTriggerStay { get => trigger.OnTriggerStayAction; set => trigger.OnTriggerStayAction = value; }
        public Action<Entity> OnTriggerExit { get => trigger.OnTriggerExitAction; set => trigger.OnTriggerExitAction = value; }

        private TriggerEntity trigger;

        public TriggerComponent(Vector2 localPosition, float width, float height, List<Type> triggerers,
            Action<Entity> OnTriggerEnter, Action<Entity> OnTriggerStay, Action<Entity> OnTriggerExit)
        {
            LocalPosition = localPosition;
            trigger = new TriggerEntity(localPosition, new Vector2(width, height), triggerers, OnTriggerEnter,
                OnTriggerStay, OnTriggerExit);
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
    }
}
