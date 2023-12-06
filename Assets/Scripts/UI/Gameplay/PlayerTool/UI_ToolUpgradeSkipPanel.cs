/*
 * Date: November 15th, 2023
 * Author: Peche
 */

using DG.Tweening;
using LittleLooters.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_ToolUpgradeSkipPanel : MonoBehaviour
    {
		public enum SkipPanelOpeningReason { NONE, TOOL_UPGRADE, REPAIR }

		#region Inspector

		[SerializeField] private GameObject _content = default;
		[SerializeField] private GameObject _panel = default;
		[SerializeField] private TextMeshProUGUI _txtTitle = default;
		[SerializeField] private Button _btnWatchAd = default;
		[SerializeField] private Button _btnInstant = default;
		[SerializeField] private Button _btnClose = default;
		[SerializeField] private PlayerEntryPoint _playerEntryPoint = default;

		[Header("Progress")]
		[SerializeField] private TextMeshProUGUI _txtTime = default;
		[SerializeField] private Slider _progressBar = default;

		[Header("Animation")]
		[SerializeField] private float _animationDuration = default;
		[SerializeField] private float _animationEndValue = default;
		[SerializeField] private float _animationDelay = default;
		[SerializeField] private Ease _animationEase = default;

		#endregion

		#region Private properties

		private int _objectId = -1;
		private bool _inProgress = false;
		private float _expiration = default;
		private float _duration = default;
		private SkipPanelOpeningReason _openingReason = SkipPanelOpeningReason.NONE;
		private float _remainingTime = 0;

		#endregion

		#region Unity events

		private void Awake()
		{
			UI_GameplayEvents.OnSkipToolUpgrade += SkipToolUpgrade;
			PlayerProgressEvents.OnMeleeUpgradeCompleted += ToolUpgradeCompleted;

			UI_GameplayEvents.OnSkipRepairing += HandleSkipRepair;
			PlayerProgressEvents.OnCompleteRepairing += HandleRepairCompleted;

			_btnWatchAd.onClick.AddListener(WatchAd);
			_btnInstant.onClick.AddListener(InstantUpgrade);
			_btnClose.onClick.AddListener(Close);

			Hide();
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnSkipToolUpgrade -= SkipToolUpgrade;
			PlayerProgressEvents.OnMeleeUpgradeCompleted -= ToolUpgradeCompleted;

			UI_GameplayEvents.OnSkipRepairing -= HandleSkipRepair;
			PlayerProgressEvents.OnCompleteRepairing -= HandleRepairCompleted;

			_btnWatchAd.onClick.RemoveAllListeners();
			_btnInstant.onClick.RemoveAllListeners();
			_btnClose.onClick.RemoveAllListeners();
		}

		private void Update()
		{
			if (!_inProgress) return;

			RefreshProgress();

			RefreshProgressBar();
		}

		#endregion

		#region Private methods

		private void Hide()
		{
			_content.SetActive(false);
		}

		[ContextMenu("TEST")]
		private void Show()
		{
			_content.SetActive(true);

			AnimatePanel();
		}

		private void WatchAd()
		{
			// TODO: SFX

			if (_openingReason == SkipPanelOpeningReason.TOOL_UPGRADE)
			{
				SkipToolUpgradeByWatchingAd();
			}
			else if (_openingReason == SkipPanelOpeningReason.REPAIR)
			{
				SkipRepairByWatchingAd();
			}

			Hide();
		}

		private void InstantUpgrade()
		{
			// TODO: SFX

			// TODO

			Debug.LogError("TODO");
		}

		private void Close()
		{
			// TODO: play SFX

			//_inProgress = false;

			Hide();
		}

		private void AnimatePanel()
		{
			_panel.transform.DOPunchScale(Vector3.one * _animationEndValue, _animationDuration).SetEase(_animationEase).SetDelay(_animationDelay);
		}

		private void RefreshProgress()
		{
			var now = Time.time;
			var remainingTime = Mathf.CeilToInt(_expiration - now);

			var timeToShow = UI_Utils.GetFormatTime(remainingTime);

			_txtTime.text = $"{timeToShow}";
		}

		private void RefreshProgressBar()
		{
			_remainingTime -= Time.deltaTime;

			var progress = (float)_remainingTime / (float)_duration;

			_progressBar.value = progress;
		}

		#endregion

		#region Tool upgrade

		private void SkipToolUpgrade()
		{
			_expiration = _playerEntryPoint.ProgressData.meleeData.upgradeExpiration;
			_duration = _playerEntryPoint.GetMeleeNextLevelData().upgradeTime;

			var now = Time.time;
			_remainingTime = _expiration - now;

			_openingReason = SkipPanelOpeningReason.TOOL_UPGRADE;

			_inProgress = true;

			_txtTitle.text = "SKIP TOOL UPGRADE"; // TODO: localize

			Show();
		}

		private void ToolUpgradeCompleted()
		{
			if (_openingReason != SkipPanelOpeningReason.TOOL_UPGRADE) return;

			_openingReason = SkipPanelOpeningReason.NONE;

			_inProgress = false;

			Hide();
		}

		private void SkipToolUpgradeByWatchingAd()
		{
			UI_GameplayEvents.OnSpeedUpToolUpgrade?.Invoke();
		}

		#endregion

		#region Repair

		private void HandleSkipRepair(int objectId, float expiration, int duration)
		{
			_objectId = objectId;
			_expiration = expiration;
			_duration = duration;

			var now = Time.time;
			_remainingTime = expiration - now;

			_openingReason = SkipPanelOpeningReason.REPAIR;

			_inProgress = true;

			_txtTitle.text = "SKIP REPAIR";	// TODO: localize

			Show();
		}

		private void HandleRepairCompleted(int id)
		{
			if (_openingReason != SkipPanelOpeningReason.REPAIR) return;

			_openingReason = SkipPanelOpeningReason.NONE;

			_inProgress = false;

			Hide();
		}

		private void SkipRepairByWatchingAd()
		{
			UI_GameplayEvents.OnSpeedUpRepairing?.Invoke(_objectId);
		}

		#endregion

	}
}