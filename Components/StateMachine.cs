using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class StateMachine<T> : Component where T : Enum
    {
        public T CurrentState;

        private Dictionary<T, Tuple<Action, Action, Action>> stateFuncs = 
            new Dictionary<T, Tuple<Action, Action, Action>>();

        private T previousState;


        public StateMachine(T currentState)
        {
            CurrentState = currentState;
        }

        public override void Update()
        {
            if(!previousState.Equals(CurrentState))
            {
                if(stateFuncs.TryGetValue(previousState, out var stateExit))
                    stateExit.Item3?.Invoke();
                if (stateFuncs.TryGetValue(CurrentState, out var stateEnter))
                    stateEnter.Item1?.Invoke();
            }

            previousState = CurrentState;

            if (stateFuncs.TryGetValue(CurrentState, out var stateUpdate))
                stateUpdate.Item2?.Invoke();
        }

        public void RegisterStateFunctions(T state, Action onEnter, Action update, Action onExit)
        {
            if (stateFuncs.ContainsKey(state))
                stateFuncs[state] = Tuple.Create(onEnter, update, onExit);
            else
                stateFuncs.Add(state, Tuple.Create(onEnter, update, onExit));
        }

        public void Switch(T state)
            => CurrentState = state;

        public bool Is(T state)
            => CurrentState.Equals(state);

        public bool HasChanged()
            => !previousState.Equals(CurrentState);

        public override string ToString()
            => $"StateMachine: {CurrentState}";
    }
}
