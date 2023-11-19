/*
 * Date: November 15th, 2023
 * Author: Peche
 */

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_CurrentMissionPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private Transform _animatedPanel = default;
		[SerializeField] private TextMeshProUGUI _txtDetail = default;
        [SerializeField] private TextMeshProUGUI _txtProgress = default;
        [SerializeField] private Image _fillProgress = default;
		[SerializeField] private Image _iconType = default;
		[SerializeField] private MissionIconsData _iconsData = default;
		[SerializeField] private UI_CurrentMissionExtraInfoPanel _extraInfoPanel = default;
		[SerializeField] private Button _btnAssistance = default;

		[Header("Complete info")]
		[SerializeField] private GameObject _bgCompleted = default;
		[SerializeField] private GameObject _iconCompleted = default;
		[SerializeField] private GameObject _iconInfo = default;

		[Header("Complete animation punch")]
		[SerializeField] private float _animationPunchDelay = default;
		[SerializeField] private float _animationPunchGoal = default;
		[SerializeField] private float _animationPunchDuration = default;
		[SerializeField][Range(1, 10)] private int _animationPunchVibrato = default;
		[SerializeField][Range(0, 1)] private float _animationPunchElasticity = default;

		[Header("Complete animation in")]
		[SerializeField] private float _animationInGoal = default;
		[SerializeField] private float _animationInDuration = default;
		[SerializeField] private float _animationInDelay = default;

		[Header("Complete animation out")]
		[SerializeField] private float _animationOutGoal = default;
		[SerializeField] private float _animationOutDuration = default;
		[SerializeField] private float _animationOutDelay = default;
		[SerializeField] private Ease _animationOutEase = default;

		[Header("Interaction animation")]
		[SerializeField] private float _animationInteractionGoal = default;
		[SerializeField] private float _animationInteractionDuration = default;
		[SerializeField] [Range(1, 10)] private int _animationInteractionVibrato = default;
		[SerializeField] [Range(0, 1)] private float _animationInteractionElasticity = default;
		[SerializeField] private Ease _animationInteractionEase = default;

		#endregion

		private bool _wasCompleted = false;

		#region Unity

		private void Awake()
		{
			PlayerMissionsEvents.OnInitialization += Initialization;
			PlayerMissionsEvents.OnMoveToMission += MoveToMission;
			PlayerMissionsEvents.OnMissionProgress += MissionProgress;

			_btnAssistance.onClick.AddListener(Assistance);
		}

		private void OnDestroy()
		{
			PlayerMissionsEvents.OnInitialization -= Initialization;
			PlayerMissionsEvents.OnMoveToMission -= MoveToMission;
			PlayerMissionsEvents.OnMissionProgress -= MissionProgress;

			_btnAssistance.onClick.RemoveAllListeners();
		}

		#endregion

		#region Private methods

		private void Initialization(MissionConfigurationData data)
		{
			RefreshMission(data);
		}

		private void MoveToMission(MissionConfigurationData data)
		{
			// Show completion of previous and then refresh it
			ShowMissionComplete();

			AnimateCompletionIn(data);
		}

		private void MissionProgress(int current, int total)
		{
			RefreshProgress(current, total);
		}

		private void RefreshMission(MissionConfigurationData data)
		{
			_txtDetail.text = data.Description;

			_iconType.sprite = _iconsData.GetIcon(data.Type);

			var goal = data.GetProgressGoal();

			RefreshProgress(0, goal);

			_extraInfoPanel.Refresh(data);
		}

		private void RefreshProgress(int current, int goal)
		{
			var progress = (float)current / goal;

			_txtProgress.text = $"{current}/{goal}";

			_fillProgress.fillAmount = progress;

			_wasCompleted = progress >= 1;
		}

		private void ShowMissionComplete()
		{
			_iconInfo.SetActive(false);
			_fillProgress.fillAmount = 1;
			_iconCompleted.SetActive(true);
			_bgCompleted.SetActive(true);
		}

		private void AnimateCompletionIn(MissionConfigurationData data)
		{
			_animatedPanel.DOPunchScale(Vector3.one * _animationPunchGoal, _animationPunchDuration, _animationPunchVibrato, _animationPunchElasticity).SetDelay(_animationPunchDelay);

			_animatedPanel.DOLocalMoveX(_animationInGoal, _animationInDuration).SetDelay(_animationInDelay).OnComplete(
				() => 
				{
					OnAnimationInCompleted(data);
				}
			);
		}

		private void OnAnimationInCompleted(MissionConfigurationData data)
		{
			HideMissionComplete();

			RefreshMission(data);

			AnimateCompletionOut();
		}

		private void HideMissionComplete()
		{
			_iconInfo.SetActive(true);
			_fillProgress.fillAmount = 0;
			_iconCompleted.SetActive(false);
			_bgCompleted.SetActive(false);
		}

		private void AnimateCompletionOut()
		{
			_animatedPanel.DOLocalMoveX(_animationOutGoal, _animationOutDuration).SetDelay(_animationOutDelay).SetEase(_animationOutEase);
		}

		private void Assistance()
		{
			// Check if mission was completed
			if (_wasCompleted) return;

			// TODO: play SFX

			AnimatePanelInteraction();

			UI_GameplayEvents.OnMissionAssistance?.Invoke();
		}

		private void AnimatePanelInteraction()
		{
			_animatedPanel.DOPunchScale(Vector3.one * _animationInteractionGoal, _animationInteractionDuration, _animationInteractionVibrato, _animationInteractionElasticity).SetEase(_animationInteractionEase);
		}

		#endregion

		#region Test

		[Header("Test")]
		[SerializeField] private MissionConfigurationData _dataTest = default;

		[ContextMenu("TEST")]
		private void TestMissionCompleted()
		{
			MoveToMission(_dataTest);
		}

		#endregion
	}
}