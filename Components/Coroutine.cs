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
        private Func<bool> pausedUntil;

        private Stack<IEnumerator> stack;

        public Coroutine(IEnumerator enumerator) 
        {
            Enumerator = enumerator;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerators">The Enumerators in order of execution</param>
        public Coroutine(params IEnumerator[] enumerators)
        {
            stack = new Stack<IEnumerator>(enumerators.Reverse());
            Enumerator = stack.Pop();
        }

        public Coroutine(Stack<IEnumerator> stack)
        {
            this.stack = stack;
            Enumerator = stack.Pop();
        }

        public struct WaitForSeconds
        {
            public float Time;
            public WaitForSeconds(float time) { Time = time; }
        }

        public struct PausedUntil
        {
            public Func<bool> Until;
            public PausedUntil(Func<bool> pausedUntil) { Until = pausedUntil; }
        }

        public override void Update()
        {
            if (pausedUntil != null && !pausedUntil())
                return;
            else
                pausedUntil = null;

            if (waitTimer > 0)
            {
                if (isTimer)
                    waitTimer -= Engine.Deltatime;
                else
                    waitTimer--;
            }
            else if (Enumerator.MoveNext())
            {
                if (Enumerator.Current == null)
                {
                    waitTimer = 0;
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
                else if (Enumerator.Current is PausedUntil paused)
                {
                    pausedUntil = paused.Until;
                    waitTimer = 0;
                    isTimer = false;
                }
            }
            else if (stack != null && stack.Count > 0)
                Enumerator = stack.Pop();
            else
                Destroy();
        }

        public static IEnumerator WaitFrames(float frames)
        {
            yield return frames;
        }

        public static IEnumerator WaitSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        public static IEnumerator WaitUntil(Func<bool> Until)
        {
            yield return new PausedUntil(Until);
        }
    }
}
