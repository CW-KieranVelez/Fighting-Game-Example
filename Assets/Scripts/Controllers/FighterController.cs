using FightTest.Data;
using FightTest.Input;
using FightTest.StateMachine;
using FightTest.States;
using FightTest.Systems;
using UnityEngine;

namespace FightTest.Controllers
{
    public class FighterController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _inputProviderBehaviour;
        [SerializeField] private CharacterStats _stats;
        [SerializeField] private CharacterMover _mover;
        [SerializeField] private FacingSystem _facing;
        [SerializeField] private LayerMask _hitLayer;

        [Header("Collider Sets")]
        [SerializeField] private ColliderSet _idleColliders;

        [SerializeField] private ColliderSet _walkColliders;
        [SerializeField] private ColliderSet _sprintColliders;
        [SerializeField] private ColliderSet _dashColliders;
        [SerializeField] private ColliderSet _crouchColliders;
        [SerializeField] private ColliderSet _crouchWalkColliders;
        [SerializeField] private ColliderSet _blockColliders;
        [SerializeField] private ColliderSet _crouchBlockColliders;
        [SerializeField] private ColliderSet _hitStunColliders;
        [SerializeField] private ColliderSet _airHitStunColliders;
        [SerializeField] private ColliderSet _knockedDownColliders;
        [SerializeField] private ColliderSet _airKnockedDownColliders;
        [SerializeField] private ColliderSet _jumpRiseColliders;
        [SerializeField] private ColliderSet _airborneColliders;
        [SerializeField] private ColliderSet _lightColliders;
        [SerializeField] private ColliderSet _heavyColliders;
        [SerializeField] private ColliderSet _throwColliders;
        [SerializeField] private ColliderSet _crouchLightColliders;
        [SerializeField] private ColliderSet _crouchHeavyColliders;
        [SerializeField] private ColliderSet _airLightColliders;
        [SerializeField] private ColliderSet _airHeavyColliders;
        [SerializeField] private ColliderSet _airThrowColliders;

        [Header("Ground Detection")]
        [SerializeField] private GroundDetector _groundDetector;

        private AirbornState _airborn;
        private SimpleState _airborne;
        private AttackState _airHeavyAttack;
        private HitStunState _airHitStun;
        private KnockedDownState _airKnockedDown;
        private AttackState _airLightAttack;
        private ThrowAttackState _airThrowAttack;
        private BlockStunState _block;
        private SimpleState _crouch;
        private BlockStunState _crouchBlock;
        private AttackState _crouchHeavyAttack;
        private AttackState _crouchLightAttack;
        private MovingState _crouchWalk;
        private DashState _dash;

        private InputFrame _frame;

        // Major states
        private GroundState _ground;
        private AttackState _heavyAttack;
        private HitStunState _hitStun;

        // Ground substates
        private SimpleState _idle;
        private IInputProvider _inputProvider;

        // Air substates
        private SimpleState _jumpRise;
        private KnockedDownState _knockedDown;
        private AttackState _lightAttack;
        private Rigidbody2D _rb;
        private StateMachine.StateMachine _root;
        private MovingState _sprint;
        private ThrowAttackState _throwAttack;
        private MovingState _walk;

        private bool IsGrounded => _groundDetector != null && _groundDetector.IsGrounded;
        private bool IsWalkingBack => _frame.MoveX * _facing.Sign < 0f;
        private bool IsMovingForward => _frame.MoveX * _facing.Sign > 0f;
        private bool IsAirborne => _root.CurrentState == _airborn;

        private bool IsGroundSubstateAttack
        {
            get
            {
                var state = _ground.SubMachine.CurrentState;
                return state == _lightAttack ||
                       state == _heavyAttack ||
                       state == _throwAttack ||
                       state == _crouchLightAttack ||
                       state == _crouchHeavyAttack;
            }
        }

        private bool IsHitStunned => _ground.SubMachine.CurrentState == _hitStun;

        private bool IsInvulnerable
        {
            get
            {
                if (_root.CurrentState == _ground)
                {
                    return _ground.SubMachine.CurrentState == _knockedDown;
                }

                if (_root.CurrentState == _airborn)
                {
                    return _airborn.SubMachine.CurrentState == _airKnockedDown;
                }

                return false;
            }
        }

        // Exposed for HitHandler — state queries
        public bool QueryIsInvulnerable => IsInvulnerable;
        public bool QueryIsAirborne => IsAirborne;
        public bool QueryIsWalkingBack => IsWalkingBack;
        public bool QueryIsInWalkState => _ground.SubMachine.CurrentState == _walk;
        public bool QueryIsInCrouchState => _ground.SubMachine.CurrentState == _crouch;

        // Exposed for HitHandler — dependencies
        public CharacterHealth Health { get; private set; }

        public CharacterMover Mover => _mover;
        public FacingSystem Facing => _facing;
        public HitStunTimer HitStunTimer { get; private set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            Health = GetComponent<CharacterHealth>();
            _inputProvider = _inputProviderBehaviour as IInputProvider;

            Health.Init(_stats.MaxHealth);

            BuildStates();
            RegisterTransitions();

            _root.Init(_ground);
        }

        private void FixedUpdate()
        {
            _frame = _inputProvider?.GetFrame() ?? default;
            _walk.MoveX = _frame.MoveX;
            _walk.Speed = IsWalkingBack ? _stats.WalkBackSpeed : _stats.MoveSpeed;
            _crouchWalk.MoveX = IsWalkingBack ? 0f : _frame.MoveX;
            _sprint.MoveX = _frame.MoveX;
            _dash.MoveX = _frame.MoveX;

            _root.Tick();
        }

        // Exposed for HitHandler — transition callbacks
        public void OnGroundHit()
        {
            _ground.SubMachine.ChangeState(_hitStun);
        }

        public void OnAirHit()
        {
            _airborn.SubMachine.ChangeState(_airHitStun);
        }

        public void OnGroundBlock(int duration)
        {
            _block.Configure(duration);
            _ground.SubMachine.ChangeState(_block);
        }

        public void OnCrouchBlock(int duration)
        {
            _crouchBlock.Configure(duration);
            _ground.SubMachine.ChangeState(_crouchBlock);
        }

        public void OnThrowLaunch(bool shouldLaunch)
        {
            if (shouldLaunch)
            {
                _airborn.ConfigureAsLaunched();
            }

            if (IsAirborne)
            {
                _airborn.SubMachine.ChangeState(_airKnockedDown);
            }
        }

        public void OnGroundKnockdown()
        {
            _ground.SubMachine.ChangeState(_knockedDown);
        }

        private void BuildStates()
        {
            HitStunTimer = new HitStunTimer();

            _idle = new SimpleState(_idleColliders);
            _crouch = new SimpleState(_crouchColliders);
            _jumpRise = new SimpleState(_jumpRiseColliders);
            _airborne = new SimpleState(_airborneColliders);

            _walk = new MovingState(_mover, _stats.MoveSpeed, _walkColliders);
            _sprint = new MovingState(_mover, _stats.SprintSpeed, _sprintColliders);
            _crouchWalk = new MovingState(_mover, _stats.MoveSpeed, _crouchWalkColliders);

            _dash = new DashState(_mover, _stats.BackDashSpeed, _stats.BackDashDuration, _dashColliders);
            _block = new BlockStunState(_blockColliders);
            _crouchBlock = new BlockStunState(_crouchBlockColliders);
            _hitStun = new HitStunState(_hitStunColliders, HitStunTimer);
            _airHitStun = new HitStunState(_airHitStunColliders, HitStunTimer);
            _knockedDown = new KnockedDownState(_knockedDownColliders,75);
            _airKnockedDown = new KnockedDownState(_airKnockedDownColliders, 0);

            _throwAttack = new ThrowAttackState(_stats.ThrowAttack, _throwColliders, _hitLayer, gameObject);
            _airThrowAttack = new ThrowAttackState(_stats.ThrowAttack, _airThrowColliders, _hitLayer, gameObject);
            _lightAttack = new AttackState(_stats.LightAttack, _lightColliders, _hitLayer, _facing, gameObject,
                "LightAttack", _mover);
            _heavyAttack = new AttackState(_stats.HeavyAttack, _heavyColliders, _hitLayer, _facing, gameObject,
                "HeavyAttack", _mover);
            _crouchLightAttack = new AttackState(_stats.CrouchLightAttack, _crouchLightColliders, _hitLayer, _facing,
                gameObject, "CrouchLight", _mover);
            _crouchHeavyAttack = new AttackState(_stats.CrouchHeavyAttack, _crouchHeavyColliders, _hitLayer, _facing,
                gameObject, "CrouchHeavy", _mover);
            _airLightAttack = new AttackState(_stats.AirLightAttack, _airLightColliders, _hitLayer, _facing, gameObject,
                "AirLight", _mover);
            _airHeavyAttack = new AttackState(_stats.AirHeavyAttack, _airHeavyColliders, _hitLayer, _facing, gameObject,
                "AirHeavy", _mover);

            _ground = new GroundState(_idle);
            _airborn = new AirbornState(_jumpRise, _mover, _stats.JumpForce);
            _root = new StateMachine.StateMachine();
        }

        private void RegisterTransitions()
        {
            _root.RegisterTransitions(
                _ground,
                new Transition(() => IsGrounded && _frame.Jump && !IsGroundSubstateAttack && !IsHitStunned, () =>
                {
                    _airborn.Configure(_frame.MoveX * _stats.MoveSpeed);
                    return _airborn;
                })
            );

            _root.RegisterTransitions(
                _airborn,
                new Transition(() => IsGrounded && _rb.velocity.y <= 0.1f, () =>
                {
                    var airSubState = _airborn.SubMachine.CurrentState;

                    if (airSubState == _airKnockedDown)
                        _ground.SubMachine.ChangeState(_knockedDown);
                    else if (airSubState == _airHitStun)
                        _ground.SubMachine.ChangeState(_hitStun);
                    else
                        _ground.SubMachine.ChangeState(_idle);

                    return _ground;
                })
            );

            var groundSm = _ground.SubMachine;

            groundSm.RegisterTransitions(
                _idle,
                new Transition(() => _frame.BackDash && IsWalkingBack, () => _dash),
                new Transition(
                    () => _frame.MoveX != 0f && _frame is { Duck: false, LightAttack: false, HeavyAttack: false },
                    () => _walk),
                new Transition(() => _frame.Duck, () => _crouch),
                new Transition(() => _frame.LightAttack, () => _lightAttack),
                new Transition(() => _frame.HeavyAttack, () => _heavyAttack),
                new Transition(() => _frame.Throw, () => _throwAttack)
            );

            groundSm.RegisterTransitions(
                _walk,
                new Transition(() => _frame.BackDash && IsWalkingBack, () => _dash),
                new Transition(() => _frame.Sprint && !_frame.Duck && IsMovingForward, () => _sprint),
                new Transition(() => _frame is { MoveX: 0f, Duck: false }, () => _idle),
                new Transition(() => _frame.Duck && (_frame.MoveX == 0f || IsWalkingBack), () => _crouch),
                new Transition(() => _frame.Duck && IsMovingForward, () => _crouchWalk),
                new Transition(() => _frame.LightAttack, () => _lightAttack),
                new Transition(() => _frame.HeavyAttack, () => _heavyAttack),
                new Transition(() => _frame.Throw, () => _throwAttack)
            );

            groundSm.RegisterTransitions(
                _sprint,
                new Transition(() => !IsMovingForward || !_frame.Sprint, () => _idle),
                new Transition(() => _frame.Duck, () => _crouch),
                new Transition(() => _frame.LightAttack, () => _lightAttack),
                new Transition(() => _frame.HeavyAttack, () => _heavyAttack),
                new Transition(() => _frame.Throw, () => _throwAttack)
            );

            groundSm.RegisterTransitions(
                _crouch,
                new Transition(() => _frame is { Duck: false, MoveX: 0f }, () => _idle),
                new Transition(() => !_frame.Duck && _frame.MoveX != 0f, () => _walk),
                new Transition(() => _frame.Duck && IsMovingForward, () => _crouchWalk),
                new Transition(() => _frame.LightAttack, () => _crouchLightAttack),
                new Transition(() => _frame.HeavyAttack, () => _crouchHeavyAttack)
            );

            groundSm.RegisterTransitions(
                _crouchWalk,
                new Transition(() => !_frame.Duck && _frame.MoveX == 0f, () => _idle),
                new Transition(() => !_frame.Duck && _frame.MoveX != 0f, () => _walk),
                new Transition(() => _frame.Duck && (_frame.MoveX == 0f || IsWalkingBack), () => _crouch),
                new Transition(() => _frame.LightAttack, () => _crouchLightAttack),
                new Transition(() => _frame.HeavyAttack, () => _crouchHeavyAttack)
            );

            groundSm.RegisterTransitions(_dash,
                new Transition(() => _dash.IsFinished, () => _idle));

            groundSm.RegisterTransitions(_block,
                new Transition(() => _block.IsFinished, () => _idle));

            groundSm.RegisterTransitions(_crouchBlock,
                new Transition(() => _crouchBlock.IsFinished, () => _crouch));

            groundSm.RegisterTransitions(_lightAttack,
                new Transition(() => _lightAttack.IsFinished, () => _idle));

            groundSm.RegisterTransitions(_heavyAttack,
                new Transition(() => _heavyAttack.IsFinished, () => _idle));

            groundSm.RegisterTransitions(_throwAttack,
                new Transition(() => _throwAttack.IsFinished, () => _idle));

            groundSm.RegisterTransitions(_crouchLightAttack,
                new Transition(() => _crouchLightAttack.IsFinished, () => _crouch));

            groundSm.RegisterTransitions(_crouchHeavyAttack,
                new Transition(() => _crouchHeavyAttack.IsFinished, () => _crouch));

            groundSm.RegisterTransitions(_hitStun,
                new Transition(() => _hitStun.IsFinished, () => _idle));

            groundSm.RegisterTransitions(_knockedDown,
                new Transition(() => _knockedDown.IsFinished, () => _idle));

            var airSm = _airborn.SubMachine;

            airSm.RegisterTransitions(
                _jumpRise,
                new Transition(() => _rb.velocity.y <= 0f, () => _airborne),
                new Transition(() => _frame.LightAttack, () => _airLightAttack),
                new Transition(() => _frame.HeavyAttack, () => _airHeavyAttack),
                new Transition(() => _frame.Throw, () => _airThrowAttack)
            );

            airSm.RegisterTransitions(
                _airborne,
                new Transition(() => _frame.LightAttack, () => _airLightAttack),
                new Transition(() => _frame.HeavyAttack, () => _airHeavyAttack),
                new Transition(() => _frame.Throw, () => _airThrowAttack)
            );

            airSm.RegisterTransitions(_airLightAttack,
                new Transition(() => _airLightAttack.IsFinished, () => _airborne));

            airSm.RegisterTransitions(_airHeavyAttack,
                new Transition(() => _airHeavyAttack.IsFinished, () => _airborne));

            airSm.RegisterTransitions(_airThrowAttack,
                new Transition(() => _airThrowAttack.IsFinished, () => _airborne));

            airSm.RegisterTransitions(_airHitStun,
                new Transition(() => _airHitStun.IsFinished, () => _airborne));
        }
    }
}