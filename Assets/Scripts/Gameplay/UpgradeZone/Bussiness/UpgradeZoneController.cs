/*
 * Date: November 5th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.UI;
using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay
{
    public class UpgradeZoneController : MonoBehaviour
    {
		#region Inspector

        [SerializeField] private GameObject _indicator = default;
		[SerializeField] private UI_UpgradeZone_Panel _uiPanel = default;
		[SerializeField] private UI_UpgradeZone_ClaimPanel _uiClaimPanel = default;
		[SerializeField] private UI_UpgradeZone_InProgressPanel _uiInProgressPanel = default;
		[SerializeField] private Transform _pivotAssistance = default;

		[Header("UI progress")]
		[SerializeField] private GameObject _uiInProgress = default;
		[SerializeField] private Slider _progressBarFill = default;

		[Header("UI completion")]
		[SerializeField] private GameObject _uiCompleted = default;

		#endregion

		#region Private properties

		private bool _isUpgrading = false;
		private bool _shouldBeClaimed = false;
		private bool _playerInsideArea = false;
		private float _duration = 0;
		private float _remainingTime = 0;

		#endregion

		#region Public properties

		public Transform PivotAssistance => _pivotAssistance;

		#endregion

		#region Unity events

		private void Awake()
		{
			SubscribeEvents();
		}

		private void OnDestroy()
		{
			UnsubscribeEvents();
		}

		private void Update()
		{
			if (_playerInsideArea) return;

			if (!_isUpgrading) return;

			RefreshUpgradeProgressBar();
		}

		#endregion

		#region Public methods

		public void ShowIndicator()
		{
			var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();

			_uiClaimPanel.Hide();
			_uiPanel.Hide();
			//_uiInProgressPanel.Hide();

			_indicator.SetActive(true);

			var toolIsUpgrading = progressDataService.ProgressData.toolData.isUpgrading;

			if (toolIsUpgrading)
			{
				//_uiInProgressPanel.Show();

				UI_GameplayEvents.OnShowUpgradeToolProgress?.Invoke();

				return;
			}

			var isMeleeClaimExpected = progressDataService.ProgressData.toolData.toClaim;

			if (isMeleeClaimExpected)
			{
				// Show claim panel
				//_uiClaimPanel.Show();

				UI_GameplayEvents.OnShowUpgradeToolClaim?.Invoke();

				return;
			}

			var currentLevelData = progressDataService.Tool_GetCurrentLevelData();
			var nextLevelData = progressDataService.Tool_GetNextLevelData();

			//_uiPanel.Show(progressDataService.ProgressData.resourcesData, currentLevelData, nextLevelData);
			UI_GameplayEvents.OnShowUpgradeToolInformation?.Invoke();
		}

		public void HideIndicator()
		{
			_indicator.SetActive(false);

			_uiPanel.Hide();

			UI_GameplayEvents.OnHideUpgradeTool?.Invoke();
		}

		#endregion

		#region Private methods

		private void SubscribeEvents()
		{
			UI_GameplayEvents.OnShowUpgradeToolInformation += HandleShowInformationPanel;
			UI_GameplayEvents.OnShowUpgradeToolProgress += HandleShowProgressPanel;
			UI_GameplayEvents.OnShowUpgradeToolClaim += HandleShowClaimPanel;
			UI_GameplayEvents.OnHideUpgradeTool += HandleHidePanel;
			PlayerProgressEvents.OnMeleeUpgradeStarted += HandleToolUpgradeStarted;
			PlayerProgressEvents.OnMeleeUpgradeCompleted += HandleToolUpgradeCompleted;
			PlayerProgressEvents.OnToolUpgradeExpirationHasChanged += HandleToolUpgradeExpirationChanged;
			PlayerProgressEvents.OnMeleeUpgradeClaimed += HandleToolUpgradeClaimed;
		}

		private void UnsubscribeEvents()
		{
			UI_GameplayEvents.OnShowUpgradeToolInformation -= HandleShowInformationPanel;
			UI_GameplayEvents.OnShowUpgradeToolProgress -= HandleShowProgressPanel;
			UI_GameplayEvents.OnShowUpgradeToolClaim -= HandleShowClaimPanel;
			UI_GameplayEvents.OnHideUpgradeTool -= HandleHidePanel;
			PlayerProgressEvents.OnMeleeUpgradeStarted -= HandleToolUpgradeStarted;
			PlayerProgressEvents.OnMeleeUpgradeCompleted -= HandleToolUpgradeCompleted;
			PlayerProgressEvents.OnToolUpgradeExpirationHasChanged -= HandleToolUpgradeExpirationChanged;
			PlayerProgressEvents.OnMeleeUpgradeClaimed -= HandleToolUpgradeClaimed;
		}

		private void HandleToolUpgradeClaimed(PlayerProgressEvents.MeleeUpgradeClaimedArgs args)
		{
			_isUpgrading = false;
			_shouldBeClaimed = false;

			HideAllPanels();
		}

		private void HandleShowInformationPanel()
		{
			_playerInsideArea = true;

			HideAllPanels();
		}

		private void HandleShowProgressPanel()
		{
			_playerInsideArea = true;

			HideAllPanels();
		}

		private void HandleShowClaimPanel()
		{
			_playerInsideArea = true;

			HideAllPanels();
		}

		private void HandleHidePanel()
		{
			_playerInsideArea = false;

			HideAllPanels();

			RefreshUIPanels();
		}

		private void HandleToolUpgradeStarted(PlayerProgressEvents.MeleeUpgradeStartedArgs args)
		{
			_isUpgrading = true;
			_shouldBeClaimed = false;

			_remainingTime = args.duration;
			_duration = args.duration;

			HideAllPanels();
		}

		private void HandleToolUpgradeCompleted()
		{
			_isUpgrading = false;
			_shouldBeClaimed = true;

			_remainingTime = 0;
			_duration = 0;

			HideAllPanels();

			if (_playerInsideArea) return;

			RefreshUIPanels();
		}

		private void HandleToolUpgradeExpirationChanged(PlayerProgressEvents.ToolUpgradeExpirationChangedArgs args)
		{
			HideAllPanels();

			var wasCompleted = Time.time >= args.expiration;

			_isUpgrading = !wasCompleted;
			_shouldBeClaimed = wasCompleted;

			if (wasCompleted)
			{
				_remainingTime = 0;
				_duration = 0;
			}
			else
			{
				_remainingTime = args.expiration - Time.time;
				_duration = args.duration;
			}

			if (_playerInsideArea) return;

			RefreshUIPanels();
		}

		private void HideAllPanels()
		{
			_uiCompleted.SetActive(false);
			_uiInProgress.SetActive(false);
		}

		private void RefreshUIPanels()
		{
			if (_isUpgrading)
			{
				_uiInProgress.SetActive(true);
				return;
			}

			if (_shouldBeClaimed)
			{
				_uiCompleted.SetActive(true);
				return;
			}
		}

		private void RefreshUpgradeProgressBar()
		{
			_remainingTime -= Time.deltaTime;

			var progress = 1 - (_remainingTime / _duration);

			_progressBarFill.value = progress;
		}

		#endregion
	}
}