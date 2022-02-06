using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class TriggerEntity : Trigger
    {
        public Action<Entity> OnTriggerEnterAction;
        public Action<Entity> OnTriggerStayAction;
        public Action<Entity> OnTriggerExitAction;

        public TriggerEntity(Vector2 position, Vector2 size, List<Type> triggerers, Action<Entity> OnTriggerEnter,
            Action<Entity> OnTriggerStay, Action<Entity> OnTriggerExit) : base(position, size, triggerers, null)
        {
            OnTriggerEnterAction = OnTriggerEnter;
            OnTriggerStayAction = OnTriggerStay;
            OnTriggerExitAction = OnTriggerExit;
        }

        public override void OnTriggerEnter(Entity entity)
            => OnTriggerEnterAction?.Invoke(entity);

        public override void OnTriggerStay(Entity entity)
            => OnTriggerStayAction?.Invoke(entity);

        public override void OnTriggerExit(Entity entity)
            => OnTriggerExitAction?.Invoke(entity);
    }
}
