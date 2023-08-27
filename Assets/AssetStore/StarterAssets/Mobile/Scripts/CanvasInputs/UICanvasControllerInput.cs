using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;

        [SerializeField] private bool _isAim = false;

        private readonly Vector2 _vectorUp = Vector2.up;

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
    }

}
