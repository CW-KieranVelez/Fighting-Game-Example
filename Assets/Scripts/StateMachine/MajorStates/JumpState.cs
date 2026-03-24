using FightTest.StateMachine;
using FightTest.Systems;

namespace FightTest.States
{
    public sealed class AirbornState : ICompositeState
    {
        private readonly IState _defaultSubState;
        private readonly float _jumpForce;
        private readonly CharacterMover _mover;
        private float _directionX;
        private bool _suppressJump;

        public AirbornState(IState defaultSubState, CharacterMover mover, float jumpForce)
        {
            _mover = mover;
            _jumpForce = jumpForce;
            _defaultSubState = defaultSubState;
            SubMachine.Init(defaultSubState);
        }

        public StateMachine.StateMachine SubMachine { get; } = new StateMachine.StateMachine();

        public void Enter()
        {
            SubMachine.ChangeState(_defaultSubState);
            if (!_suppressJump)
            {
                _mover.Jump(_jumpForce, _directionX);
            }
        }

        public void Tick()
        {
        }

        public void Exit()
        {
        }

        public void Configure(float directionX)
        {
            _directionX = directionX;
            _suppressJump = false;
        }

        public void ConfigureAsLaunched()
        {
            _suppressJump = true;
        }
    }
}