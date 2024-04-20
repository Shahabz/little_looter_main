/*
 * Date: April 13th, 2024
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.InputSystem;

namespace LittleLooters.Gameplay
{
    public class SwipeDetectionService : MonoBehaviour
    {
		[SerializeField] private InputAction _positionAction = default;
		[SerializeField] private InputAction _pressAction = default;

		private Vector2 _startTouchPosition = default;
		private float _startTime = 0;
		private Vector2 _endTouchPosition = default;
		private float _endTime = 0;
		private float _maxSwipeDuration = 0.3f;
		private float _minSwipeDistance = 100000;

		private void Awake()
		{
			_positionAction.Enable();
			_pressAction.Enable();
			_pressAction.performed += HandlePerformedAction;
			_pressAction.canceled += HandleCanceledAction;
		}

		private void OnDestroy()
		{
			_positionAction.Disable();
			_pressAction.Disable();
			_pressAction.performed -= HandlePerformedAction;
			_pressAction.canceled -= HandleCanceledAction;
		}

		public void RefreshTime(float value)
		{
			_maxSwipeDuration = value;
		}

		public void RefreshDistance(float value)
		{
			_minSwipeDistance = value;
		}

		private void ProcessSwipe()
		{
			DebugSwipe();

			var swipeDuration = _endTime - _startTime;

			if (swipeDuration > _maxSwipeDuration)
			{
				ClearAll();
				return;
			}

			var dir = (_endTouchPosition - _startTouchPosition);
			var distance = dir.sqrMagnitude;

			if (distance < _minSwipeDistance)
			{
				ClearAll();
				return;
			}

			var dirNormalized = dir.normalized;

			UI_GameplayEvents.OnProcessSwipe?.Invoke(dirNormalized);

			ClearAll();
		}

		private void ClearAll()
		{
			_endTouchPosition = default;
			_startTouchPosition = default;
		}

		private void HandleCanceledAction(InputAction.CallbackContext context)
		{
			Debug.LogError("Cancel swipe!");
			_endTime = Time.time;
			_endTouchPosition = _positionAction.ReadValue<Vector2>();

			ProcessSwipe();
		}

		private void HandlePerformedAction(InputAction.CallbackContext context)
		{
			Debug.LogError("Start swipe!");
			_startTime = Time.time;
			_startTouchPosition = _positionAction.ReadValue<Vector2>();
		}

		private void DebugSwipe()
		{
			var swipeDuration = _endTime - _startTime;
			var dir = (_endTouchPosition - _startTouchPosition);
			var distance = dir.sqrMagnitude;

			Debug.LogError("---- Swipe ----");
			Debug.LogError($"Swipe duration {swipeDuration}, max duration: {_maxSwipeDuration}");
			Debug.LogError($"Swipe distance: {distance}, min distance: {_minSwipeDistance}");
		}
	}
}