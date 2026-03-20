namespace FightTest.StateMachine
{
    public interface ICompositeState : IState
    {
        StateMachine SubMachine { get; }
    }
}
