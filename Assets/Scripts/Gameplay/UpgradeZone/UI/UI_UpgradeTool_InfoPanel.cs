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
		[SerializeField] private Color _colorTextUpgradeEnable = default;
		[SerializeField] private Color _colorTextUpgradeDisable = default;
		[SerializeField] private Color _colorBackgroundUpgradeEnable = default;
		[SerializeField] private Color _colorBackgroundUpgradeDisable = default;
		[SerializeField] private GameObject _btnUpgradeAlert = default;

		#endregion

		#region Unity events

		private void Awake()
		{
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
			/*
			HideSlots();

			LoadInformation();

            _content.SetActive(true);
			*/
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

				slot.Setup(playerResource.info.Id, playerResource.amount, requirement.amount, requirement.resource.Icon);

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
			_btnUpgrade.enabled = canUpgrade;

			_btnUpgradeBackground.color = (canUpgrade) ? _colorBackgroundUpgradeEnable : _colorBackgroundUpgradeDisable;

			_txtUpgradeButton.color = (canUpgrade) ? _colorTextUpgradeEnable : _colorTextUpgradeDisable;

			_btnUpgradeAlert.SetActive(canUpgrade);

			if (!canUpgrade) return;
		}

		private void StartUpgrade()
		{
			// TODO: play SFX

			// Hide popup
			Hide();

			// Notify about start upgrading
			UI_GameplayEvents.OnStartToolUpgrade?.Invoke();

			AnimateConsumedResources();
		}

		private void AnimateConsumedResources()
		{
			var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();
			var playerResourcesData = progressDataService.ProgressData.resourcesData;
			var toolNextLevelData = progressDataService.Tool_GetNextLevelData();
			var resourceIds = new int[toolNextLevelData.requirements.Length];

			for (int i = 0; i < toolNextLevelData.requirements.Length; i++)
			{
				resourceIds[i] = toolNextLevelData.requirements[i].resource.Id;
			}

			var resourceAmounts = new int[toolNextLevelData.requirements.Length];

			for (int i = 0; i < toolNextLevelData.requirements.Length; i++)
			{
				resourceAmounts[i] = toolNextLevelData.requirements[i].amount;
			}

			var position = _btnUpgradeBackground.rectTransform.position;
			position.y += 50;

			var args = new UI_ResourcesAnimation.ResourceConsumptionArgs()
			{
				ids = resourceIds,
				amounts = resourceAmounts,
				position = position
			};

			UI_ResourcesAnimation.OnAnimateResourceConsumption?.Invoke(args);
		}

		#endregion
	}
}