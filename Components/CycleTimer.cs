using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class CycleTimer : Component
    {
        private Timer timer;
        public int? NumCycles;

        private int cycle;

        public Action<Timer> OnUpdate;
        public Action<int> OnCycle;

        public CycleTimer(float time, int? numberOfCycles, Action<Timer> updateAction, Action<int> onCycle)
        {
            OnUpdate = updateAction;
            OnCycle = onCycle;

            timer = new Timer(time, true, (timer) => OnUpdate?.Invoke(timer), () => { onCycle?.Invoke(0); RestartTimer(); });
            NumCycles = numberOfCycles;
        }

        public override void Added()
        {
            base.Added();

            ParentEntity.AddComponent(timer);
        }

        private void RestartTimer()
        {
            if (NumCycles == null || cycle < NumCycles)
            {
                cycle++;
                timer = new Timer(timer.MaxValue, true, (timer) => OnUpdate?.Invoke(timer), () => { OnCycle?.Invoke(cycle); RestartTimer(); });
            }
            else
                ParentEntity.RemoveComponent(this);
        }
    }
}
