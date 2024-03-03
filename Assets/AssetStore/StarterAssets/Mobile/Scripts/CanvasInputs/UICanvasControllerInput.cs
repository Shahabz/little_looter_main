using LittleLooters.Gameplay;
using System;
using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {
		#region Inspector

		[Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;
        [SerializeField] private bool _isAim = false;
        [SerializeField] private UIVirtualButton _btnSprint = default;
        [SerializeField] private UIVirtualButton _btnRepair = default;
        [SerializeField] private UIVirtualButton _btnFire = default;
        [SerializeField] private UIVirtualButton _btnReload = default;
        [SerializeField] private float _deadZoneThreshold = 0.5f;
        [SerializeField] private PlayerRepairService _playerRepairService = default;
        [SerializeField] private VariableJoystick _joystick = default;

        #endregion

        #region Private properties

        private bool _gameStarted = false;
        private readonly Vector2 _vectorUp = Vector2.up;
        private bool _sprintState = false;
        private bool _repairState = false;

        #endregion

        #region Unity events

        private void Awake()
        {
            UI_GameplayEvents.OnStartGame += HandleStartGame;
        }

        private void Start()
		{
            starterAssetsInputs.OnCancelSprint += CancelSprint;

            if (_joystick != null)
            {
                _joystick.OnChange += HandleJoystickChange;
                _joystick.OnStop += HandleJoystickStop;
            }

            if (_btnRepair == null) return;

            _playerRepairService.OnDetectTarget += DetectRepairTarget;
            _playerRepairService.OnUndetectTarget += UndetectRepairTarget;
        }

        private void OnDestroy()
        {
            UI_GameplayEvents.OnStartGame -= HandleStartGame;

            starterAssetsInputs.OnCancelSprint -= CancelSprint;

            if (_joystick != null)
            {
                _joystick.OnChange -= HandleJoystickChange;
                _joystick.OnStop -= HandleJoystickStop;
            }

            if (_btnRepair == null) return;

            _playerRepairService.OnDetectTarget -= DetectRepairTarget;
            _playerRepairService.OnUndetectTarget -= UndetectRepairTarget;
        }

		#endregion

		#region Public methods

		public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            if (_isAim)
			{
                // Check Dead zone for X axis
                if (Mathf.Abs(virtualMoveDirection.x) < _deadZoneThreshold)
                {
                    virtualMoveDirection.x = 0;
                }

                // Check Dead zone for X axis
                if (Mathf.Abs(virtualMoveDirection.y) < _deadZoneThreshold)
                {
                    virtualMoveDirection.y = 0;
                }

                starterAssetsInputs.AimInput(virtualMoveDirection);
                return;
			}

            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            starterAssetsInputs.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput()
        {
            _sprintState = !_sprintState;

            starterAssetsInputs.SprintInput(_sprintState);
        }

        public void VirtualFireInput()
        {
            if (!_gameStarted) return;

            starterAssetsInputs.AttackInput(_btnFire.StatusOn);
        }

        public void VirtualReloadInput()
        {
            starterAssetsInputs.ReloadInput(_btnReload.StatusOn);
        }

        public void VirtualRepairInput()
        {
            _repairState = !_repairState;

            starterAssetsInputs.RepairInput(_repairState);
        }

        public void VirtualRollingInput()
        {
            if (!_gameStarted) return;

            starterAssetsInputs.RollInput();
        }

        #endregion

        #region Private methods

        private void HandleStartGame()
		{
            _gameStarted = true;
		}

        private void CancelSprint()
		{
            if (_btnSprint == null) return;

            _btnSprint.Deactivate();

            _sprintState = false;
		}

        private void DetectRepairTarget()
		{
            /*_btnRepair.Deactivate();
            _btnRepair.gameObject.SetActive(true);

            _repairState = false;*/
		}

        private void UndetectRepairTarget()
        {
            _btnRepair.gameObject.SetActive(false);
            _btnRepair.Deactivate();

            _repairState = false;

            starterAssetsInputs.repair = false;
        }

        private void HandleJoystickChange(Vector2 direction)
        {
            if (!_gameStarted) return;

            starterAssetsInputs.MoveInput(direction);
        }

        private void HandleJoystickStop()
		{
            if (!_gameStarted) return;

            starterAssetsInputs.MoveInput(Vector2.zero);
        }

        #endregion
    }

}
