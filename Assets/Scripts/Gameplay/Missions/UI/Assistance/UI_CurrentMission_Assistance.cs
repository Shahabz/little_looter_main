/*
 * Date: November 18th, 2023
 * Author: Peche
 */

using UnityEngine;
using DG.Tweening;

namespace LittleLooters.Gameplay.UI
{
	public class UI_CurrentMission_Assistance : MonoBehaviour
	{
		#region Inspector

		[SerializeField] private PlayerEntryPoint _playerEntryPoint = default;
		[SerializeField] private GameObject _indicator = default;
		[SerializeField] private UI_FollowCamera _followCameraComponent = default;
		[SerializeField] private Transform _target = default;
		[SerializeField] private Vector3 _targetOffset = default;
		[SerializeField] private Transform _indicatorIcon = default;
		[SerializeField] private float _animationScale = default;
		[SerializeField] private float _animationDuration = default;
		[SerializeField] private float _animationDelay = default;
		[SerializeField] private GameObject _indicatorOffScreen = default;
		[SerializeField] private float _assistanceDuration = default;
		[SerializeField] private float _animationVerticalOffset = default;
		[SerializeField] private float _animationVerticalDuration = default;
		[SerializeField] private float _animationVerticalDelay = default;

		#endregion

		#region Private properties

		private Sequence _tweenSequence = default;
		private Sequence _tweenSequenceVerticalMovement = default;
		private MissionDestructionAssistance _destructionAssistance = default;
		private MissionUpgradeToolAssistance _upgradeToolAssistance = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			UI_GameplayEvents.OnMissionAssistance += StartAssistance;

			_destructionAssistance = GetComponent<MissionDestructionAssistance>();
			_upgradeToolAssistance = GetComponent<MissionUpgradeToolAssistance>();

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
			UI_GameplayEvents.OnMissionAssistance -= StartAssistance;

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

			_indicator.transform.SetParent(target);
			_indicator.transform.localScale = Vector3.one * 0.01f;
			_indicator.transform.localPosition = Vector3.zero + targetOffset;

			_followCameraComponent.enabled = true;

			_indicator.SetActive(true);

			StartIndicatorAnimation();

			Invoke(nameof(StopAssistance), _assistanceDuration);
		}

		private void StopAssistance()
		{
			_indicator.SetActive(false);

			_followCameraComponent.enabled = false;

			_indicator.transform.SetParent(transform);
			_indicator.transform.localScale = Vector3.one;
			_indicator.transform.localPosition = Vector3.zero;

			StopIndicatorAnimation();
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

			return (target, offset, found);
		}

		#endregion
	}
}