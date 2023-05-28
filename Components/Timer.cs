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

        public Action OnComplete;
        public Action<Timer> UpdateAction;
        
        public Timer(float maxValue, bool destroyOnComplete = true, Action<Timer> UpdateAction = null, Action OnComplete = null)
        {
            this.MaxValue = maxValue;
            Value = maxValue;
            this.OnComplete = OnComplete;
            this.UpdateAction = UpdateAction;
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
                    OnComplete?.Invoke();
                    if (destroyOnComplete)
                        ParentEntity.RemoveComponent(this);
                }
                else
                    UpdateAction?.Invoke(this);
            }

            if (Paused && PausedFunc())
            {
                Paused = false;
                PausedFunc = () => false;
            }
        }

        public void End()
        {
            OnComplete?.Invoke();
            ParentEntity.RemoveComponent(this);
        }

        public void PauseUntil(Func<bool> pausedUntil)
        {
            if (pausedUntil == null)
                return;
            PausedFunc = pausedUntil;
            Paused = true;
        }

        public float AmountCompleted()
            => Ease.Reverse(Value / MaxValue);
    }
}