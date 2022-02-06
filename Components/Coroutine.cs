using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class Coroutine : Component
    {
        public IEnumerator Enumerator;
        private float waitTimer;
        private bool isTimer;

        public Coroutine(IEnumerator enumerator) 
        {
            Enumerator = enumerator;
        }

        public struct WaitForSeconds
        {
            public float Time;
            public WaitForSeconds(float time) { Time = time; }
        }

        public override void Update()
        {
            if (waitTimer > 0)
            {
                if (isTimer)
                    waitTimer -= Engine.Deltatime;
                else
                    waitTimer--;
            }
            else if (Enumerator.MoveNext())
            {
                Enumerator.MoveNext();
                if (Enumerator.Current is int)
                {
                    waitTimer = (int)Enumerator.Current;
                    isTimer = false;
                }
                else if (Enumerator.Current is int)
                {
                    waitTimer = (int)Enumerator.Current;
                    isTimer = false;
                }
                else if (Enumerator.Current is WaitForSeconds wait)
                {
                    waitTimer = wait.Time;
                    isTimer = true;
                }
            }
            else
                Destroy();
        }
    }
}
