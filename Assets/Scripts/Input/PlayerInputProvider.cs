using System;
using FightTest.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace FightTest.Input
{
    public enum ControlScheme
    {
        KeyboardMouse,
        Gamepad
    }

    public class PlayerInputProvider : MonoBehaviour, IInputProvider, PlayerControls.INewactionmapActions
    {
        private const float DoubleTapWindow = 0.25f;
        private const int BufferTicks = 10;

        [SerializeField] private ControlScheme _scheme = ControlScheme.KeyboardMouse;
        [SerializeField] private int _gamepadIndex;

        private PlayerControls _controls;
        private int _heavyBuffer;
        private float _lastTapDir;
        private float _lastTapTime = -1f;

        private int _lightBuffer;
        private Vector2 _move;
        private float _prevMoveX;
        private bool _sprinting;
        private bool _backDash;
        private InputUser _user;

        private void Awake()
        {
            _controls = new PlayerControls();
            _controls.Disable();
            BindToScheme();
            _controls.Newactionmap.SetCallbacks(this);
        }

        private void OnEnable()
        {
            _controls?.Newactionmap.Enable();
        }

        private void OnDisable()
        {
            _controls.Newactionmap.Disable();
        }

        private void OnDestroy()
        {
            _user.UnpairDevicesAndRemoveUser();
            _controls.Dispose();
        }

        public InputFrame GetFrame()
        {
            UpdateSprint();

            if (_lightBuffer > 0)
            {
                _lightBuffer--;
            }

            if (_heavyBuffer > 0)
            {
                _heavyBuffer--;
            }

            var backDash = _backDash;
            _backDash = false;
            return new InputFrame(_move.x, _move.y, _lightBuffer > 0, _heavyBuffer > 0, _sprinting, backDash);
        }

        public void OnMoveDirection(InputAction.CallbackContext ctx)
        {
            _move = ctx.ReadValue<Vector2>();
        }

        public void OnLightAttack(InputAction.CallbackContext ctx)
        {
            if (ctx.phase == InputActionPhase.Performed)
            {
                _lightBuffer = BufferTicks;
            }
        }

        public void OnHeavyAttack(InputAction.CallbackContext ctx)
        {
            if (ctx.phase == InputActionPhase.Performed)
            {
                _heavyBuffer = BufferTicks;
            }
        }

        public (int light, int heavy) GetBufferSnapshot()
        {
            return (_lightBuffer, _heavyBuffer);
        }

        private void UpdateSprint()
        {
            var x = _move.x;
            var wasZero = Mathf.Approximately(_prevMoveX, 0f);
            var isNonZero = !Mathf.Approximately(x, 0f);

            if (isNonZero && wasZero)
            {
                var dir = Mathf.Sign(x);
                if (Mathf.Approximately(dir, _lastTapDir) && Time.time - _lastTapTime <= DoubleTapWindow)
                {
                    _sprinting = true;
                    _backDash = true;
                }

                _lastTapTime = Time.time;
                _lastTapDir = dir;
            }

            if (Mathf.Approximately(x, 0f))
            {
                _sprinting = false;
            }

            _prevMoveX = x;
        }

        private void BindToScheme()
        {
            _user = InputUser.CreateUserWithoutPairedDevices();
            _user.AssociateActionsWithUser(_controls);

            switch (_scheme)
            {
                case ControlScheme.KeyboardMouse:
                    if (Keyboard.current != null)
                    {
                        _user = InputUser.PerformPairingWithDevice(Keyboard.current, _user);
                    }

                    if (Mouse.current != null)
                    {
                        _user = InputUser.PerformPairingWithDevice(Mouse.current, _user);
                    }

                    _user.ActivateControlScheme("KeyboardMouse");
                    break;

                case ControlScheme.Gamepad:
                    var gamepads = Gamepad.all;
                    if (_gamepadIndex < gamepads.Count)
                    {
                        _user = InputUser.PerformPairingWithDevice(gamepads[_gamepadIndex], _user);
                        _user.ActivateControlScheme("Gamepad");
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"[PlayerInputProvider] Gamepad index {_gamepadIndex} not found. No device bound.");
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}