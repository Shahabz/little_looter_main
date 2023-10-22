using LittleLooters.Gameplay;
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
        [SerializeField] private float _deadZoneThreshold = 0.5f;
        [SerializeField] private PlayerRepairService _playerRepairService = default;

        #endregion

        #region Private properties

        private readonly Vector2 _vectorUp = Vector2.up;
        private bool _sprintState = false;
        private bool _repairState = false;

        #endregion

        #region Unity events

        private void Start()
		{
            starterAssetsInputs.OnCancelSprint += CancelSprint;

            if (_btnRepair == null) return;

            _playerRepairService.OnDetectTarget += DetectRepairTarget;
            _playerRepairService.OnUndetectTarget += UndetectRepairTarget;
        }

        private void OnDestroy()
        {
            starterAssetsInputs.OnCancelSprint -= CancelSprint;

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
            starterAssetsInputs.AttackInput();
        }

        public void VirtualReloadInput()
        {
            starterAssetsInputs.ReloadInput();
        }

        public void VirtualRepairInput()
        {
            _repairState = !_repairState;

            starterAssetsInputs.RepairInput(_repairState);
        }

        #endregion

        #region Private methods

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

        #endregion
    }

}
