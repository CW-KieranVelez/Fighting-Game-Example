using System.Collections.Generic;

namespace FightTest.StateMachine
{
    public class StateMachine
    {
        private static readonly List<ITransition> _empty = new List<ITransition>();

        private readonly Dictionary<IState, List<ITransition>> _transitions =
            new Dictionary<IState, List<ITransition>>();

        public IState CurrentState { get; private set; }

        public void Init(IState state)
        {
            CurrentState = state;
            state.Enter();

            if (state is ICompositeState composite)
            {
                composite.SubMachine.Init(composite.SubMachine.CurrentState);
            }
        }

        public void Tick()
        {
            if (CurrentState == null)
            {
                return;
            }

            foreach (var transition in GetTransitions(CurrentState))
            {
                var next = transition.Evaluate();
                if (next == null)
                {
                    continue;
                }

                ChangeState(next);
                return;
            }

            CurrentState.Tick();

            if (CurrentState is ICompositeState composite)
            {
                composite.SubMachine.Tick();
            }
        }

        public void ChangeState(IState next)
        {
            ExitDeep(CurrentState);
            CurrentState = next;
            next.Enter();

            if (next is ICompositeState enteringComposite)
            {
                enteringComposite.SubMachine.Init(enteringComposite.SubMachine.CurrentState);
            }
        }

        private static void ExitDeep(IState state)
        {
            if (state == null)
            {
                return;
            }

            if (state is ICompositeState composite)
            {
                ExitDeep(composite.SubMachine.CurrentState);
            }

            state.Exit();
        }

        public void RegisterTransitions(IState state, params ITransition[] transitions)
        {
            if (!_transitions.ContainsKey(state))
            {
                _transitions[state] = new List<ITransition>();
            }

            _transitions[state].AddRange(transitions);
        }

        private List<ITransition> GetTransitions(IState state)
        {
            return _transitions.TryGetValue(state, out var list) ? list : _empty;
        }
    }
}