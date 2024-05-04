/*
 * Date: March 18th, 2024
 * Author: Peche
 */

using StarterAssets;

namespace LittleLooters.Gameplay.Combat
{
    public class PlayerAutofireAssistance
    {
		#region Private properties

		private ThirdPersonController _movementController = default;
		private WeaponController _weaponController = default;
		private PlayerAimingAssistance _aimingAssistance = default;
		private bool _enabled = false;
		private bool _movementFree = false;

		#endregion

		#region Public properties

		public bool CanFireOnPause => _enabled && !_movementFree;

		#endregion

		#region Public methods

		public void Init(ThirdPersonController movementController, WeaponController weaponController, PlayerAimingAssistance aimingAssistance)
		{
			_movementController = movementController;
			_weaponController = weaponController;
			_aimingAssistance = aimingAssistance;

			UI_GameplayEvents.OnAutofireChangedByCheat += HandleAutofireChanged;
		}

		public void Teardown()
		{
			UI_GameplayEvents.OnAutofireChangedByCheat -= HandleAutofireChanged;
		}

		public void Tick()
		{
			if (!_enabled) return;

			Process();
		}

		#endregion

		#region Private methods

		private void HandleAutofireChanged(UI_GameplayEvents.AutofireByCheatArgs args)
		{
			_enabled = args.enabled;
			_movementFree = args.movementFree;
		}

		private void Process()
		{
			if (_movementFree)
			{
				ProcessMovementFree();
				return;
			}

			ProcessMovementPause();
		}

		private void ProcessMovementFree()
		{
			// Check aiming target
			if (!_aimingAssistance.TargetDetected)
			{
				_weaponController.StopAutofire();
				return;
			}

			// Check aiming target inside detection area
			if (!_aimingAssistance.TargetInsideRadius)
			{
				_weaponController.StopAutofire();
				return;
			}

			_weaponController.ApplyAutofire();
		}

		private void ProcessMovementPause()
		{
			if (_movementController.IsMoving())
			{
				_weaponController.StopAutofire();
				return;
			}

			// Check aiming target
			if (!_aimingAssistance.TargetDetected)
			{
				_weaponController.StopAutofire();
				return;
			}

			// Check aiming target inside detection area
			if (!_aimingAssistance.TargetInsideRadius)
			{
				_weaponController.StopAutofire();
				return;
			}

			_weaponController.ApplyAutofire();
		}

		#endregion
	}
}