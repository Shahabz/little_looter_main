/*
 * Date: November 18th, 2023
 * Author: Peche
 */

using UnityEngine;
using DG.Tweening;

namespace LittleLooters.Gameplay.UI
{
	/// <summary>
	/// Logic in charge to show mission indicator above the target.
	/// If target is offscreen it shows another indicator at the screen pointing towards the target.
	/// Ref: https://github.com/jinincarnate/off-screen-indicator
	/// </summary>
	public class UI_CurrentMission_Assistance : MonoBehaviour
	{
		#region Inspector

		[SerializeField] private PlayerEntryPoint _playerEntryPoint = default;
		[SerializeField] private GameObject _indicator = default;
		[SerializeField] private UI_FollowCamera _followCameraComponent = default;
		[SerializeField] private Transform _indicatorIcon = default;
		[SerializeField] private float _animationScale = default;
		[SerializeField] private float _animationDuration = default;
		[SerializeField] private float _animationDelay = default;
		[SerializeField] private float _assistanceDuration = default;
		[SerializeField] private float _animationVerticalOffset = default;
		[SerializeField] private float _animationVerticalDuration = default;
		[SerializeField] private float _animationVerticalDelay = default;

		[Header("Offscreen")]
		[SerializeField] private Camera _camera = default;
		[SerializeField] private GameObject _indicatorOffScreen = default;
		[SerializeField] private Transform _indicatorOffScreenDirection = default;
		[SerializeField] private float _verticalOffscreenOffset = default;
		[SerializeField] private float _offset = default;

		#endregion

		#region Private properties

		private Sequence _tweenSequence = default;
		private Sequence _tweenSequenceVerticalMovement = default;
		private MissionDestructionAssistance _destructionAssistance = default;
		private MissionUpgradeToolAssistance _upgradeToolAssistance = default;
		private MissionExplorationAssistance _explorationAssistance = default;
		private MissionCraftingAssistance _craftingAssistance = default;
		private MissionDeliveryAssistance _deliveryAssistance = default;
		private Transform _target = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			UI_GameplayEvents.OnTriggerMissionAssistance += StartAssistance;
			UI_GameplayEvents.OnCancelMissionAssistance += CancelAssistance;
			UI_GameplayEvents.OnStopMissionAssistance += CancelAssistance;

			_destructionAssistance = GetComponent<MissionDestructionAssistance>();
			_upgradeToolAssistance = GetComponent<MissionUpgradeToolAssistance>();
			_explorationAssistance = GetComponent<MissionExplorationAssistance>();
			_craftingAssistance = GetComponent<MissionCraftingAssistance>();
			_deliveryAssistance = GetComponent<MissionDeliveryAssistance>();

			_tweenSequence = DOTween.Sequence()
									.Append(_indicatorIcon.DOScale(Vector3.one * _animationScale, _animationDuration).SetDelay(_animationDelay))
									.Append(_indicatorIcon.DOScale(Vector2.one, _animationDuration))
									.SetLoops(-1, LoopType.Restart);

			_tweenSequence = DOTween.Sequence()
									.Append(_indicatorIcon.DOLocalMoveY(_animationVerticalOffset, _animationVerticalDuration).SetDelay(_animationVerticalDelay))
									.Append(_indicatorIcon.DOLocalMoveY(0, _animationVerticalDuration))
									.SetLoops(-1, LoopType.Restart);
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnTriggerMissionAssistance -= StartAssistance;
			UI_GameplayEvents.OnCancelMissionAssistance -= CancelAssistance;
			UI_GameplayEvents.OnStopMissionAssistance -= CancelAssistance;

			_tweenSequence = null;

			_tweenSequenceVerticalMovement = null;

			CancelInvoke();
		}

		#endregion

		#region Private methods

		private void StartIndicatorAnimation()
		{
			_tweenSequence.Restart();
			_tweenSequenceVerticalMovement.Restart();
		}

		private void StopIndicatorAnimation()
		{
			_tweenSequence.Pause();
			_tweenSequenceVerticalMovement.Pause();
		}

		private void StartAssistance()
		{
			CancelInvoke(nameof(StopAssistance));

			var (target, targetOffset, found) = GetTargetInfo();

			if (!found) return;

			_target = target;

			_indicator.transform.SetParent(target);
			_indicator.transform.localScale = Vector3.one * 0.01f;
			_indicator.transform.localPosition = Vector3.zero + targetOffset;

			_followCameraComponent.enabled = true;

			_indicator.SetActive(true);

			StartIndicatorAnimation();

			StartIndicatorOffScreen();

			Invoke(nameof(StopAssistance), _assistanceDuration);
		}

		private void StopAssistance()
		{
			CancelAssistance();

			UI_GameplayEvents.OnMissionAssistanceFinished?.Invoke();
		}

		private (Transform target, Vector3 targetOffset, bool found) GetTargetInfo()
		{
			Transform target = null;
			Vector3 offset = Vector3.zero;
			var found = false;

			var currentMission = _playerEntryPoint.GetMissionInProgress();

			if (currentMission.Type == MissionType.DESTRUCTION)
			{
				var missionInfo = (MissionResourceDestructionData)currentMission;

				return _destructionAssistance.Process(currentMission, missionInfo.Destructible.Id);
			}

			if (currentMission.Type == MissionType.TOOL_UPGRADE)
			{
				return _upgradeToolAssistance.Process(currentMission, _playerEntryPoint.ProgressData);
			}

			if (currentMission.Type == MissionType.EXPLORATION)
			{
				var missionInfo = (MissionExplorationData)currentMission;

				return _explorationAssistance.Process(currentMission, missionInfo.explorableType);
			}

			if (currentMission.Type == MissionType.CRAFTING)
			{
				var missionInfo = (MissionCraftingData)currentMission;

				return _craftingAssistance.Process(missionInfo.ResourceData.Id);
			}

			if (currentMission.Type == MissionType.DELIVERY)
			{
				var missionInfo = (MissionDeliveryData)currentMission;

				return _deliveryAssistance.Process(missionInfo.RepairObjectData.Id);
			}

			return (target, offset, found);
		}

		private void StartIndicatorOffScreen()
		{
			_indicatorOffScreen.SetActive(true);

			InvokeRepeating(nameof(RefreshIndicatorOffScreen), 0, 0.1f);
		}

		private void StopIndicatorOffScreen()
		{
			_indicatorOffScreen.SetActive(false);

			CancelInvoke(nameof(RefreshIndicatorOffScreen));
		}

		private void RefreshIndicatorOffScreen()
		{
			var targetPosition = _target.position;
			targetPosition.y = 0;
			var targetScreenPosition = _camera.WorldToScreenPoint(targetPosition);
			
			// Check if target is on screen
			var isTargetVisible = IsTargetVisible(targetScreenPosition);

			if (isTargetVisible)
			{
				_indicatorOffScreen.SetActive(false);
				return;
			}

			_indicatorOffScreen.SetActive(true);

			var playerPosition = _playerEntryPoint.transform.position;
			playerPosition.y = 0;
			var playerScreenPosition = _camera.WorldToScreenPoint(playerPosition);
			playerScreenPosition.y = _verticalOffscreenOffset;

			Vector3 targetDirection = (targetScreenPosition - playerScreenPosition).normalized;

			var angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

			_indicatorOffScreenDirection.transform.localEulerAngles = new Vector3(0, 0, angle);
		}

		/// <summary>
		/// Gets if the target is within the view frustrum.
		/// </summary>
		/// <param name="screenPosition">Position of the target mapped to screen cordinates</param>
		/// <returns></returns>
		private bool IsTargetVisible(Vector3 screenPosition)
		{
			bool isTargetVisible = screenPosition.z > 0 && (screenPosition.x > _offset) && screenPosition.x < (Screen.width - _offset) && (screenPosition.y > _offset) && screenPosition.y < (Screen.height - _offset);

			return isTargetVisible;
		}

		private void CancelAssistance()
		{
			_indicator.SetActive(false);

			_followCameraComponent.enabled = false;

			_indicator.transform.SetParent(transform);
			_indicator.transform.localScale = Vector3.one;
			_indicator.transform.localPosition = Vector3.zero;

			StopIndicatorAnimation();

			StopIndicatorOffScreen();
		}

		#endregion
	}
}