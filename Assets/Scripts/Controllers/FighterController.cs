using FightTest.Data;
using FightTest.Input;
using FightTest.StateMachine;
using FightTest.States;
using FightTest.Systems;
using UnityEngine;

namespace FightTest.Controllers
{
    public class FighterController : MonoBehaviour, IHittable
    {
        [SerializeField] private MonoBehaviour _inputProviderBehaviour;
        [SerializeField] private CharacterStats _stats;
        [SerializeField] private CharacterMover _mover;
        [SerializeField] private FacingSystem _facing;

        [Header("Collider Sets")]
        [SerializeField] private ColliderSet _idleColliders;

        [SerializeField] private ColliderSet _walkColliders;
        [SerializeField] private ColliderSet _sprintColliders;
        [SerializeField] private ColliderSet _crouchColliders;
        [SerializeField] private ColliderSet _crouchWalkColliders;
        [SerializeField] private ColliderSet _blockColliders;
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
        [SerializeField] private ColliderSet _dashColliders;
        [SerializeField] private LayerMask _hitLayer;

        [Header("Ground Detection")]
        [SerializeField] private GroundDetector _groundDetector;

        private AirbornState _airborn;

        private AirborneState _airborne;
        private AttackState _airHeavyAttack;
        private HitStunState _airHitStun;
        private AirKnockedDownState _airKnockedDown;
        private AttackState _airLightAttack;
        private ThrowAttackState _airThrowAttack;
        private DashState _dash;
        private BlockState _block;
        private CrouchState _crouch;
        private AttackState _crouchHeavyAttack;
        private AttackState _crouchLightAttack;
        private CrouchWalkState _crouchWalk;
        private InputFrame _frame;

        private GroundState _ground;
        private CharacterHealth _health;
        private AttackState _heavyAttack;
        private HitStunState _hitStun;
        private HitStunTimer _hitStunTimer;
        private IdleState _idle;

        private IInputProvider _inputProvider;
        private JumpRiseState _jumpRise;
        private KnockedDownState _knockedDown;

        private bool _landKnockedDown;

        private AttackState _lightAttack;
        private Rigidbody2D _rb;
        private StateMachine.StateMachine _root;
        private SprintState _sprint;
        private ThrowAttackState _throwAttack;
        private WalkState _walk;

        private bool IsGrounded => _groundDetector != null && _groundDetector.IsGrounded;
        private bool IsWalkingBack => _frame.MoveX * _facing.Sign < 0f;
        private bool IsMovingForward => _frame.MoveX * _facing.Sign > 0f;

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

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _health = GetComponent<CharacterHealth>();
            _inputProvider = _inputProviderBehaviour as IInputProvider;

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

        public void ReceiveHit(AttackData data)
        {
            if (IsInvulnerable)
            {
                Debug.Log("invulnerable!");
                Debug.Log(_ground.SubMachine.CurrentState == _knockedDown);
                Debug.Log(_airborn.SubMachine.CurrentState == _airKnockedDown);
                return;
            }

            if (data.Launches)
            {
                ReceiveThrow(data);
                return;
            }

            _mover.AddForce(new Vector2(-_facing.Sign * data.Knockback.x, data.Knockback.y));
            _health.TakeDamage(data.Damage);

            _hitStunTimer.Configure(data.EnemyHitStopFrames);

            if (_root.CurrentState == _airborn)
            {
                _airborn.SubMachine.ChangeState(_airHitStun);
                return;
            }

            var current = _ground.SubMachine.CurrentState;
            var canBlock = IsWalkingBack && current switch
            {
                _ when current == _walk => data.Height == AttackHeight.Mid || data.Height == AttackHeight.Air,
                _ when current == _crouch => data.Height == AttackHeight.Mid || data.Height == AttackHeight.Low,
                _ => false
            };

            if (canBlock)
            {
                _block.Configure(5);
                _ground.SubMachine.ChangeState(_block);
            }
            else
            {
                _ground.SubMachine.ChangeState(_hitStun);
            }
        }

        public void ReceiveThrow(AttackData data)
        {
            if (IsInvulnerable)
            {
                return;
            }

            _health.TakeDamage(data.Damage);
            _mover.AddForce(new Vector2(-_facing.Sign * data.Knockback.x, data.Knockback.y));

            if (data.Knockback.y > 0 || _root.CurrentState == _airborn)
            {
                _airborn.ConfigureAsLaunched();
            }

            if (_root.CurrentState == _airborn)
            {
                _airborn.SubMachine.ChangeState(_airKnockedDown);
            }

            if (_root.CurrentState == _ground)
            {
                _ground.SubMachine.ChangeState(_knockedDown);
            }
        }

        private void BuildStates()
        {
            _hitStunTimer = new HitStunTimer();

            _idle = new IdleState(_mover, _idleColliders);
            _walk = new WalkState(_mover, _stats.MoveSpeed, _walkColliders);
            _sprint = new SprintState(_mover, _stats.SprintSpeed, _sprintColliders);
            _dash = new DashState(_mover, _stats.BackDashSpeed, _stats.BackDashDuration, _dashColliders);
            _crouch = new CrouchState(_mover, _crouchColliders);
            _crouchWalk = new CrouchWalkState(_mover, _stats.MoveSpeed, _crouchWalkColliders);
            _block = new BlockState(_mover, _blockColliders);
            _hitStun = new HitStunState(_hitStunColliders, _hitStunTimer);
            _airHitStun = new HitStunState(_airHitStunColliders, _hitStunTimer);
            _knockedDown = new KnockedDownState(_knockedDownColliders);
            _airKnockedDown = new AirKnockedDownState(_airKnockedDownColliders);
            _throwAttack = new ThrowAttackState(_stats.ThrowAttack, _throwColliders, _hitLayer, _facing, gameObject);
            _airThrowAttack = new ThrowAttackState(_stats.ThrowAttack, _airThrowColliders, _hitLayer, _facing, gameObject);
            _lightAttack = new AttackState(_stats.LightAttack, _lightColliders, _hitLayer, _facing, gameObject, "LightAttack", _mover);
            _heavyAttack = new AttackState(_stats.HeavyAttack, _heavyColliders, _hitLayer, _facing, gameObject, "HeavyAttack", _mover);
            _crouchLightAttack = new AttackState(_stats.CrouchLightAttack, _crouchLightColliders, _hitLayer, _facing, gameObject, "CrouchLight", _mover);
            _crouchHeavyAttack = new AttackState(_stats.CrouchHeavyAttack, _crouchHeavyColliders, _hitLayer, _facing, gameObject, "CrouchHeavy", _mover);
            _airLightAttack = new AttackState(_stats.AirLightAttack, _airLightColliders, _hitLayer, _facing, gameObject, "AirLight", _mover);
            _airHeavyAttack = new AttackState(_stats.AirHeavyAttack, _airHeavyColliders, _hitLayer, _facing, gameObject, "AirHeavy", _mover);

            _jumpRise = new JumpRiseState(_jumpRiseColliders);
            _airborne = new AirborneState(_airborneColliders);
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
                    _landKnockedDown = _airborn.SubMachine.CurrentState == _airKnockedDown;

                    if (_airborn.SubMachine.CurrentState == _airHitStun)
                    {
                        _ground.SubMachine.ChangeState(_hitStun);
                    }
                    else
                    {
                        _ground.SubMachine.ChangeState(_idle);
                    }

                    return _ground;
                })
            );

            var groundSm = _ground.SubMachine;

            groundSm.RegisterTransitions(
                _idle,
                new Transition(() =>
                {
                    if (!_landKnockedDown)
                    {
                        return false;
                    }

                    _landKnockedDown = false;
                    return true;
                }, () => _knockedDown),
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

            groundSm.RegisterTransitions(
                _dash,
                new Transition(() => _dash.IsFinished, () => _idle)
            );

            groundSm.RegisterTransitions(
                _block,
                new Transition(() => _block.IsFinished, () => _idle)
            );

            groundSm.RegisterTransitions(
                _lightAttack,
                new Transition(() => _lightAttack.IsFinished, () => _idle)
            );

            groundSm.RegisterTransitions(
                _heavyAttack,
                new Transition(() => _heavyAttack.IsFinished, () => _idle)
            );

            groundSm.RegisterTransitions(
                _throwAttack,
                new Transition(() => _throwAttack.IsFinished, () => _idle)
            );

            groundSm.RegisterTransitions(
                _crouchLightAttack,
                new Transition(() => _crouchLightAttack.IsFinished, () => _crouch)
            );

            groundSm.RegisterTransitions(
                _crouchHeavyAttack,
                new Transition(() => _crouchHeavyAttack.IsFinished, () => _crouch)
            );

            groundSm.RegisterTransitions(
                _hitStun,
                new Transition(() => _hitStun.IsFinished, () => _idle)
            );

            groundSm.RegisterTransitions(
                _knockedDown,
                new Transition(() => _knockedDown.IsFinished, () => _idle)
            );

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

            airSm.RegisterTransitions(
                _airLightAttack,
                new Transition(() => _airLightAttack.IsFinished, () => _airborne)
            );

            airSm.RegisterTransitions(
                _airHeavyAttack,
                new Transition(() => _airHeavyAttack.IsFinished, () => _airborne)
            );

            airSm.RegisterTransitions(
                _airThrowAttack,
                new Transition(() => _airThrowAttack.IsFinished, () => _airborne)
            );

            airSm.RegisterTransitions(
                _airHitStun,
                new Transition(() => _airHitStun.IsFinished, () => _airborne)
            );
        }
    }
}