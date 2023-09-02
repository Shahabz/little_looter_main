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

		#endregion

		#region Private properties

		private readonly Vector2 _vectorUp = Vector2.up;

		#endregion

		#region Unity events

		private void Start()
		{
            starterAssetsInputs.OnCancelSprint += CancelSprint;
        }

        private void OnDestroy()
        {
            starterAssetsInputs.OnCancelSprint -= CancelSprint;
        }

		#endregion

		#region Public methods

		public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            if (_isAim)
			{
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

        public void VirtualSprintInput(bool virtualSprintState)
        {
            starterAssetsInputs.SprintInput(virtualSprintState);
        }

        public void VirtualFireInput()
        {
            starterAssetsInputs.AttackInput();
        }

        public void VirtualReloadInput()
        {
            starterAssetsInputs.ReloadInput();
        }

		#endregion

		#region Private methods

        private void CancelSprint()
		{
            if (_btnSprint == null) return;

            _btnSprint.Deactivate();
		}

		#endregion
	}

}
