/*
 * Date: March 23th, 2024
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    /// <summary>
    /// Class in charge of floating panel that shows the current tool upgrade requirements
    /// </summary>
    public class UI_UpgradeTool_FloatingInfoPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _content = default;
		[SerializeField] private TextMeshProUGUI _txtTitle = default;
		[SerializeField] private UI_UpgradeZone_Slot[] _slots = default;

		[Header("Upgrade button")]
		[SerializeField] private Button _btnUpgrade = default;
		[SerializeField] private Image _btnUpgradeBackground = default;

		#endregion

		#region Private properties

		private bool _isVisible = true;

		#endregion

		#region Unity events

		private void Awake()
		{
			UI_GameplayEvents.OnStartGame += HandleGameStarted;

			PlayerProgressEvents.OnResourceHasChanged += HandlePlayerResourcesHasChanged;
			PlayerProgressEvents.OnMeleeUpgradeClaimed += HandleToolLevelUpClaimed;

			_btnUpgrade.onClick.AddListener(ButtonAction_StartUpgrade);
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnStartGame -= HandleGameStarted;

			PlayerProgressEvents.OnResourceHasChanged -= HandlePlayerResourcesHasChanged;
			PlayerProgressEvents.OnMeleeUpgradeClaimed -= HandleToolLevelUpClaimed;

			_btnUpgrade.onClick.RemoveAllListeners();
		}

		#endregion

		#region Private methods

		private void HandleGameStarted()
		{
			Show();
		}

		private void HandlePlayerResourcesHasChanged(int resourceId, int amount)
		{
			if (!_isVisible) return;

			Show();
		}

		private void HandleToolLevelUpClaimed(PlayerProgressEvents.MeleeUpgradeClaimedArgs args)
		{
			Show();
		}

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

			_btnUpgrade.gameObject.SetActive(canUpgrade);
		}

		private void ButtonAction_StartUpgrade()
		{
			// TODO: play SFX

			// Hide popup
			Hide();

			// Notify about start upgrading
			UI_GameplayEvents.OnStartToolUpgrade?.Invoke();

			AnimateConsumedResources();
		}

		private void Show()
		{
			_isVisible = true;

			HideSlots();

			LoadInformation();

			_content.SetActive(true);
		}

		private void Hide()
		{
			_content.SetActive(false);

			_isVisible = false;
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