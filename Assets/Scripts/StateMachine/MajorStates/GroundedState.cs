using FightTest.StateMachine;

namespace FightTest.States
{
    public sealed class GroundState : ICompositeState
    {
        public GroundState(IState defaultSubState)
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