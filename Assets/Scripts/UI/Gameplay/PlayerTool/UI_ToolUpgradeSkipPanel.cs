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
		[SerializeField] private Button _btnWatchAd = default;
		[SerializeField] private Button _btnInstant = default;
		[SerializeField] private Button _btnClose = default;
		[SerializeField] private PlayerEntryPoint _playerEntryPoint = default;

		[Header("Progress")]
		[SerializeField] private TextMeshProUGUI _txtTime = default;
		[SerializeField] private Image _progressBar = default;

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
			// TODO: play SFX

			if (_openingReason == SkipPanelOpeningReason.TOOL_UPGRADE)
			{
				UI_GameplayEvents.OnSpeedUpToolUpgrade?.Invoke();
			}
			else if (_openingReason == SkipPanelOpeningReason.REPAIR)
			{
				UI_GameplayEvents.OnSpeedUpRepairing?.Invoke(_objectId);
			}

			_inProgress = false;

			_objectId = -1;

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

			_openingReason = SkipPanelOpeningReason.NONE;

			_inProgress = false;

			Hide();
		}

		private void AnimatePanel()
		{
			_panel.transform.localScale = Vector3.zero;
			_panel.transform.DOScale(_animationEndValue, _animationDuration).SetEase(_animationEase).SetDelay(_animationDelay);
		}

		private void RefreshProgress()
		{
			var now = Time.time;
			var remainingTime = _expiration - now;

			var secs = remainingTime;
			var mins = Mathf.FloorToInt(secs / 60);

			secs = Mathf.CeilToInt(secs - mins * 60);

			_txtTime.text = $"{mins:00}:{secs:00}";

			var progress = 1 - remainingTime / _duration;

			_progressBar.fillAmount = progress;
		}

		#endregion

		#region Tool upgrade

		private void SkipToolUpgrade()
		{
			_openingReason = SkipPanelOpeningReason.TOOL_UPGRADE;

			_expiration = _playerEntryPoint.ProgressData.meleeData.upgradeExpiration;
			_duration = _playerEntryPoint.GetMeleeNextLevelData().upgradeTime;

			_inProgress = true;

			Show();
		}

		private void ToolUpgradeCompleted()
		{
			if (_openingReason != SkipPanelOpeningReason.TOOL_UPGRADE) return;

			_openingReason = SkipPanelOpeningReason.NONE;

			_inProgress = false;

			Hide();
		}

		#endregion

		#region Repair

		private void HandleSkipRepair(int objectId, float expiration, int duration)
		{
			_openingReason = SkipPanelOpeningReason.REPAIR;

			_objectId = objectId;
			_expiration = expiration;
			_duration = duration;

			_inProgress = true;

			Show();
		}

		private void HandleRepairCompleted(int id)
		{
			if (_openingReason != SkipPanelOpeningReason.REPAIR) return;

			_openingReason = SkipPanelOpeningReason.NONE;

			_inProgress = false;

			Hide();
		}

		#endregion

	}
}