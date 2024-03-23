/*
 * Date: November 5th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace LittleLooters.Gameplay.UI
{
    public class UI_UpgradeZone_Panel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _panel = default;
		[SerializeField] private Canvas _canvas = default;
		[SerializeField] private TextMeshProUGUI _txtTitle = default;
		[SerializeField] private UI_UpgradeZone_Slot[] _slots = default;

		[Header("Upgrade button")]
		[SerializeField] private Button _btnUpgrade = default;
		[SerializeField] private Image _btnUpgradeBackground = default;
		[SerializeField] private TextMeshProUGUI _txtUpgradeButton = default;
		[SerializeField] private Sprite _colorUpgradeEnable = default;
		[SerializeField] private Sprite _colorUpgradeDisable = default;
		[SerializeField] private Color _colorTextUpgradeEnable = default;
		[SerializeField] private Color _colorTextUpgradeDisable = default;
		[SerializeField] private GameObject _btnUpgradeAlert = default;
		[SerializeField] private float _alertAnimationScale = default;
		[SerializeField] private float _alertAnimationDuration = default;
		[SerializeField] private TextMeshProUGUI _txtCurrentDamage = default;
		[SerializeField] private TextMeshProUGUI _txtNextDamage = default;

		#endregion

		#region Private properties

		private Sequence _tweenSequence = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			_tweenSequence = DOTween.Sequence()
								.Append(_btnUpgradeAlert.transform.DOScale(Vector3.one * _alertAnimationScale, _alertAnimationDuration).SetDelay(0.1f))
								.Append(_btnUpgradeAlert.transform.DOScale(Vector2.one, _alertAnimationDuration))
								.SetLoops(-1, LoopType.Restart);

			Hide();

			_btnUpgrade.onClick.AddListener(StartUpgrade);
		}

		private void OnDestroy()
		{
			_btnUpgrade.onClick.RemoveAllListeners();
		}

		#endregion

		#region Public methods

		public void Show(PlayerProgress_ResourcesData playerResourcesData, ConfigurationMeleeLevelData currentLevelData, ConfigurationMeleeLevelData nextLevelData)
		{
			HideSlots();

			LoadInformation(playerResourcesData, currentLevelData, nextLevelData);

			_panel.SetActive(true);
			_canvas.enabled = true;
		}

        public void Hide()
		{
			_panel.SetActive(false);
			_canvas.enabled = false;
		}

		#endregion

		#region Private methods

		private void HideSlots()
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				_slots[i].gameObject.SetActive(false);
			}
		}

		private void LoadInformation(PlayerProgress_ResourcesData playerResourcesData, ConfigurationMeleeLevelData currentLevelData, ConfigurationMeleeLevelData nextLevelData)
		{
			_txtTitle.text = $"UPGRADE TO <color=#50FC8A>LEVEL {nextLevelData.level}</color>";

			var canUpgrade = true;

			for (int i = 0; i < nextLevelData.requirements.Length; i++)
			{
				if (i >= _slots.Length) break;

				var requirement = nextLevelData.requirements[i];

				var playerResource = playerResourcesData.GetResourceAmount(requirement.resource.Id);

				var slot = _slots[i];

				if (playerResource.amount < requirement.amount)
				{
					canUpgrade = false;
				}

				slot.Setup(playerResource.info.Id, playerResource.amount, requirement.amount, requirement.resource.Icon);

				slot.gameObject.SetActive(true);
			}

			RefreshDamage(currentLevelData.damage, nextLevelData.damage);

			RefreshButtonState(canUpgrade);
		}

		private void RefreshButtonState(bool canUpgrade)
		{
			StopAlertAnimation();

			_btnUpgrade.enabled = canUpgrade;

			_btnUpgradeBackground.sprite = (canUpgrade) ? _colorUpgradeEnable : _colorUpgradeDisable;

			_txtUpgradeButton.color = (canUpgrade) ? _colorTextUpgradeEnable : _colorTextUpgradeDisable;

			_btnUpgradeAlert.SetActive(canUpgrade);

			if (!canUpgrade) return;

			StartAlertAnimation();
		}

		private void StopAlertAnimation()
		{
			_tweenSequence.Pause();
		}

		private void StartAlertAnimation()
		{
			_tweenSequence.Restart();
		}

		private void StartUpgrade()
		{
			// TODO: play SFX

			// Hide popup
			Hide();

			// Notify about start upgrading
			UI_GameplayEvents.OnStartToolUpgrade?.Invoke();
		}

		private void RefreshDamage(int current, int next)
		{
			var extraDamage = next - current;

			_txtCurrentDamage.text = $"{current}";
			_txtNextDamage.text = $"+{extraDamage}";
		}

		#endregion
	}
}