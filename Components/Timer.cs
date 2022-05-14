using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class Timer : Component
    {
        public readonly float MaxValue;
        public float Value;
        public bool Paused = false;
        public float TimeScale = 1;
        public Func<bool> PausedFunc = () => false;

        private bool destroyOnComplete;

        private Action onComplete;
        private Action<Timer> updateAction;
        
        public Timer(float maxValue, bool destroyOnComplete = true, Action<Timer> UpdateAction = null, Action OnComplete = null)
        {
            this.MaxValue = maxValue;
            Value = maxValue;
            onComplete = OnComplete;
            this.updateAction = UpdateAction;
            this.destroyOnComplete = destroyOnComplete;
        }

        public override void Update()
        {
            if(Value > 0 && !Paused)
            {
                Value -= Engine.Deltatime * TimeScale;
                if (Value <= 0)
                {
                    Value = 0;
                    onComplete?.Invoke();
                    if (destroyOnComplete)
                        ParentEntity.RemoveComponent(this);
                }
                else
                    updateAction?.Invoke(this);
            }

            if (Paused && PausedFunc())
            {
                Paused = false;
                PausedFunc = () => false;
            }
        }

        public void End()
        {
            onComplete?.Invoke();
            ParentEntity.RemoveComponent(this);
        }

        public void PauseUntil(Func<bool> pausedUntil)
        {
            PausedFunc = pausedUntil;
            Paused = true;
        }
    }
}