using LittleLooters.Gameplay;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		public System.Action OnCancelSprint;

		#region Inspector

		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool repair;
		public Vector2 aim;
		public bool attack;
		public bool reload;
		public bool roll = false;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		#endregion

		#region Public properties

		public bool IsAiming => aim != vectorZero;
		public bool IsMoving => move != vectorZero;
		public bool StartAiming => startAiming;
		public bool StopAiming => stopAiming;
		public Vector2 SwipeDirection => _swipeDirection;

		#endregion

		#region Private properties

		private bool _gameStarted = false;
		private bool startAiming = false;
		private bool stopAiming = false;
		private readonly Vector2 vectorZero = Vector2.zero;
		private Vector2 _swipeDirection = default;

		#endregion

		#region Public methods

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnAttack(InputValue value)
		{
			//if (!value.isPressed) return;

			AttackInput(value.isPressed);
		}

		public void OnReload(InputValue value)
		{
			if (!value.isPressed) return;

			ReloadInput(value.isPressed);
		}

		public void OnRoll(InputValue value)
		{
			if (!_gameStarted) return;

			if (!value.isPressed) return;

			RollInput();
		}
#endif

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void AimInput(Vector2 direction)
		{
			startAiming = false;
			stopAiming = false;

			var wasAiming = IsAiming;

			aim = direction;

			startAiming = (!wasAiming && IsAiming);

			stopAiming = (wasAiming && !IsAiming);
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void AttackInput(bool inputActive)
		{
			attack = inputActive;
		}

		public void ReloadInput(bool inputActive)
		{
			reload = inputActive;
		}

		public void CancelSprint()
		{
			sprint = false;

			OnCancelSprint?.Invoke();
		}

		public void RepairInput(bool state)
		{
			repair = state;

			Debug.LogError($"Repair input: <color=magenta>{state}</color>");
		}

		public void RollInput()
		{
			roll = true;
		}

		#endregion

		#region Unity events

		private void Awake()
		{
			UI_GameplayEvents.OnStartGame += HandleStartGame;
			UI_GameplayEvents.OnProcessSwipe += HandleSwipe;
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnStartGame -= HandleStartGame;
			UI_GameplayEvents.OnProcessSwipe -= HandleSwipe;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		#endregion

		#region Private methods

		private void HandleStartGame()
		{
			_gameStarted = true;
		}

		private void HandleSwipe(Vector2 dir)
		{
			Debug.LogError("StarterAssetsInputs::HandleSwipe");
			_swipeDirection = dir;
			RollInput();
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

		#endregion
	}

}