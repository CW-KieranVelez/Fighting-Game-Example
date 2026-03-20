using FightTest.StateMachine;

namespace FightTest.States
{
    public sealed class GroundedState : ICompositeState
    {
        public GroundedState(IState defaultSubState)
        {
            SubMachine.Init(defaultSubState);
        }

        public StateMachine.StateMachine SubMachine { get; } = new StateMachine.StateMachine();

        public void Enter()
        {
        }

        public void Tick()
        {
        }

        public void Exit()
        {
        }
    }
}