using System;

namespace FightTest.StateMachine
{
    public class Transition : ITransition
    {
        private readonly Func<bool> _condition;
        private readonly Func<IState> _factory;

        public Transition(Func<bool> condition, Func<IState> factory)
        {
            _condition = condition;
            _factory = factory;
        }

        public IState Evaluate() => _condition() ? _factory() : null;
    }
}
