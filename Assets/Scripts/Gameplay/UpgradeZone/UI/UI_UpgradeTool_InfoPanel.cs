/*
 * Date: February 29th, 2024
 * Author: Peche
 */

using DG.Tweening;
using LittleLooters.Global.ServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_UpgradeTool_InfoPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _content = default;
		[SerializeField] private TextMeshProUGUI _txtTitle = default;
		[SerializeField] private UI_UpgradeZone_Slot[] _slots = default;
		[SerializeField] private TextMeshProUGUI _txtCurrentDamage = default;
		[SerializeField] private TextMeshProUGUI _txtNextDamage = default;

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

		public void Show()
		{
			HideSlots();

			LoadInformation();

            _content.SetActive(true);
		}

        public void Hide()
		{
            _content.SetActive(false);
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

		private void LoadInformation()
		{
            var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();
            var playerResourcesData = progressDataService.ProgressData.resourcesData;
            var toolCurrentLevelData = progressDataService.Tool_GetCurrentLevelData();
            var toolNextLevelData = progressDataService.Tool_GetNextLevelData();

			_txtTitle.text = $"UPGRADE TO <color=#50FC8A>LEVEL {toolNextLevelData.level}</color>";

			var canUpgrade = true;

			for (int i = 0; i < toolNextLevelData.requirements.Length; i++)
			{
				if (i >= _slots.Length) break;

				var requirement = toolNextLevelData.requirements[i];

				var playerResource = playerResourcesData.GetResourceAmount(requirement.resource.Id);

				var slot = _slots[i];

				if (playerResource.amount < requirement.amount)
				{
					canUpgrade = false;
				}

				slot.Setup(playerResource.amount, requirement.amount, requirement.resource.Icon);

				slot.gameObject.SetActive(true);
			}

			RefreshDamage(toolCurrentLevelData.damage, toolNextLevelData.damage);

			RefreshButtonState(canUpgrade);
		}

		private void RefreshDamage(int current, int next)
		{
			_txtCurrentDamage.text = $"{current}";
			_txtNextDamage.text = $"{next}";
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

		#endregion
	}
}