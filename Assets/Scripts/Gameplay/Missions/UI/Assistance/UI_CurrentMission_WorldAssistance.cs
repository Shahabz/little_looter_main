/*
 * Date: February 23th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_CurrentMission_WorldAssistance : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private Transform _content = default;
		[SerializeField] private Transform _player = default;
		[SerializeField] private float _forwardOffset = default;
		[SerializeField] private float _verticalOffset = default;
		[SerializeField] private Transform _target = default;
		[SerializeField] private LineRenderer _lineRenderer = default;

		[Header("Animation")]
		[SerializeField] private float _animationRate = default;
		[SerializeField] private float _animationOffset = default;
		[SerializeField] private float _animationDuration = default;

		#endregion

		private bool _isAssisting = false;

		#region Unity events

		private void Awake()
		{
			PlayerMissionsEvents.OnMissionAssistanceOffStarted += HandleOnTriggerAssistance;
			PlayerMissionsEvents.OnMissionAssistanceOffStopped += HandleOnStopAssistance;
		}

		private void OnDestroy()
		{
			PlayerMissionsEvents.OnMissionAssistanceOffStarted -= HandleOnTriggerAssistance;
			PlayerMissionsEvents.OnMissionAssistanceOffStopped -= HandleOnStopAssistance;
		}

		private void LateUpdate()
		{
			if (!_isAssisting) return;

			RefreshDirection();
		}

		#endregion

		#region Private methods

		private void HandleOnTriggerAssistance(Transform target)
		{
			_target = target;

			StartAssistance();
		}

		private void HandleOnStopAssistance()
		{
			StopAssistance();
		}

		private void StartAssistance()
		{
			RefreshDirection();

			_content.gameObject.SetActive(true);

			_isAssisting = true;
		}

		private void StopAssistance()
		{
			_content.gameObject.SetActive(false);

			_isAssisting = false;
		}

		private void RefreshDirection()
		{
			var playerPosition = _player.position + Vector3.forward * _forwardOffset + Vector3.up * _verticalOffset;
			var targetPosition = _target.position;

			var direction = (targetPosition - _player.position).normalized;
			var rotation = Quaternion.LookRotation(direction);

			_content.SetPositionAndRotation(playerPosition, rotation.normalized);
		}

		#endregion
	}
}