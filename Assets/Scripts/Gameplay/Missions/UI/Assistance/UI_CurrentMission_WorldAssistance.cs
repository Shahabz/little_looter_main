/*
 * Date: February 23th, 2024
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
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

		#region Private properties

		private bool _isAssisting = false;
		private bool _isToolAssisting = false;
		private float _toolUpgradeAssistanceDuration = 5f;

		#endregion

		#region Unity events

		private void Awake()
		{
			PlayerMissionsEvents.OnMissionAssistanceOffStarted += HandleOnTriggerAssistance;
			PlayerMissionsEvents.OnMissionAssistanceOffStopped += HandleOnStopAssistance;
			UI_GameplayEvents.OnTriggerToolAreaAssistance += HandleOnTriggerToolAreaAssistance;
			UI_GameplayEvents.OnUpgradeToolAreaInteracion += HandleToolUpgradeAreaInteraction;
		}

		private void OnDestroy()
		{
			PlayerMissionsEvents.OnMissionAssistanceOffStarted -= HandleOnTriggerAssistance;
			PlayerMissionsEvents.OnMissionAssistanceOffStopped -= HandleOnStopAssistance;
			UI_GameplayEvents.OnTriggerToolAreaAssistance -= HandleOnTriggerToolAreaAssistance;
			UI_GameplayEvents.OnUpgradeToolAreaInteracion -= HandleToolUpgradeAreaInteraction;
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
			CancelInvoke(nameof(StopAssistance));

			_target = target;

			_isToolAssisting = false;

			StartAssistance();
		}

		private void HandleOnStopAssistance()
		{
			StopAssistance();
		}

		private void HandleOnTriggerToolAreaAssistance()
		{
			if (_isToolAssisting)
			{
				StopAssistance();
				return;
			}

			_isToolAssisting = true;

			var levelService = ServiceLocator.Current.Get<LevelService>();

			var toolUpgradeArea = levelService.ToolUpgradeArea;

			_target = toolUpgradeArea;

			StartAssistance();

			Invoke(nameof(StopAssistance), _toolUpgradeAssistanceDuration);
		}

		private void HandleToolUpgradeAreaInteraction()
		{
			if (!_isToolAssisting) return;

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
			CancelInvoke(nameof(StopAssistance));

			_content.gameObject.SetActive(false);

			_isAssisting = false;

			_isToolAssisting = false;
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