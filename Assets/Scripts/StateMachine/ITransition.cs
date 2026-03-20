namespace FightTest.StateMachine
{
    public interface ITransition
    {
        IState Evaluate();
    }
}