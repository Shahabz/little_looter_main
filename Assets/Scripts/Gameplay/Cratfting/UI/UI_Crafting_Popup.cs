/*
 * Date: December 16th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using LittleLooters.Model;
using System;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    /// <summary>
    /// Class in charge of showing UI panels related with Crafting area when player is near to it.
    /// </summary>
    public class UI_Crafting_Popup : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private PlayerEntryPoint _entryPoint = default;
		[SerializeField] private PlayerCraftingService _playerService = default;
        [SerializeField] private UI_Crafting_InformationPanel _infoPanel = default;
        [SerializeField] private UI_Crafting_InProgressPanel _inProgressPanel = default;
        [SerializeField] private UI_Crafting_ClaimPanel _claimPanel = default;

		#endregion

		private bool _isVisible = false;

		#region Unity events

		private void OnEnable()
		{
            PlayerCraftingEvents.OnStartAreaInteraction += HandleStartAreaInteraction;
            PlayerCraftingEvents.OnStopAreaInteraction += HandleStopAreaInteraction;

			PlayerProgressEvents.OnCraftingAreaProcessStarted += HandleOnCraftingAreaProcessStarted;
			PlayerProgressEvents.OnCraftingAreaProcessCompleted += HandleOnCraftingAreaProcessCompleted;
			PlayerProgressEvents.OnCraftingAreaProcessClaimed += HandleOnCraftingAreaProcessClaimed;
			PlayerProgressEvents.OnCraftingAreaProcessSpeedUp += HandleOnCraftingAreaProcessSpeedUp;
		}

		private void Start()
		{
			HideAllPanels();
		}

		private void OnDisable()
        {
            PlayerCraftingEvents.OnStartAreaInteraction -= HandleStartAreaInteraction;
            PlayerCraftingEvents.OnStopAreaInteraction -= HandleStopAreaInteraction;

			PlayerProgressEvents.OnCraftingAreaProcessStarted -= HandleOnCraftingAreaProcessStarted;
			PlayerProgressEvents.OnCraftingAreaProcessCompleted -= HandleOnCraftingAreaProcessCompleted;
			PlayerProgressEvents.OnCraftingAreaProcessClaimed -= HandleOnCraftingAreaProcessClaimed;
			PlayerProgressEvents.OnCraftingAreaProcessSpeedUp -= HandleOnCraftingAreaProcessSpeedUp;
		}

		#endregion

		#region Private methods

		private void HandleStartAreaInteraction(CraftingConfigurationData data)
		{
			_isVisible = true;

			var craftingStatus = _entryPoint.ProgressData.craftingData.GetAreaStatus(data.Id);

			if (craftingStatus == CraftingStatus.NONE)
			{
				ShowCraftingInformation(data);
				return;
			}

			// Check status IN PROGRESS
			if (craftingStatus == CraftingStatus.IN_PROGRESS)
			{
				var areaProgressData = _entryPoint.ProgressData.craftingData.GetAreaProgressData(data.Id);

				ShowCraftingProgress(areaProgressData);

				return;
			}

			// Check status COMPLETED
			if (craftingStatus == CraftingStatus.COMPLETED)
			{
				var areaProgressData = _entryPoint.ProgressData.craftingData.GetAreaProgressData(data.Id);

				ShowCraftingCompleted(data, areaProgressData.amount);

				return;
			}

			Debug.LogError("Crafting::StartInteraction -> error");
		}

		private void HideAllPanels()
		{
			_infoPanel.Hide();
			_inProgressPanel.Hide();
			_claimPanel.Hide();
		}

		private void HandleStopAreaInteraction()
		{
			_isVisible = false;

			HideAllPanels();
		}

		private void ShowCraftingInformation(CraftingConfigurationData data)
		{
			_infoPanel.Show(data, _entryPoint.ProgressData);

			_inProgressPanel.Hide();
			_claimPanel.Hide();
		}

		private void ShowCraftingProgress(PlayerProgress_CraftingAreaData data)
		{
			var craftingConfigurationData = _playerService.GetAreaData(data.id);

			var duration = data.amount * craftingConfigurationData.DurationByUnitInSecs;

			_inProgressPanel.Show(craftingConfigurationData.Id, craftingConfigurationData.ResourceGenerated.Icon, data.expiration, duration);
		}

		private void ShowCraftingCompleted(CraftingConfigurationData data, int amount)
		{
			HideAllPanels();

			if (!_isVisible) return;

			_claimPanel.Show(data, amount);
		}

		#endregion

		#region Player progress events Handlers

		private void HandleOnCraftingAreaProcessStarted(PlayerProgress_CraftingAreaData data)
		{
			HideAllPanels();

			ShowCraftingProgress(data);
		}

		private void HandleOnCraftingAreaProcessCompleted(PlayerProgress_CraftingAreaData data)
		{
			var configurationData = _playerService.GetAreaData(data.id);

			ShowCraftingCompleted(configurationData, data.amount);
		}

		private void HandleOnCraftingAreaProcessClaimed(PlayerProgress_CraftingAreaData data)
		{
			HideAllPanels();

			var configurationData = _playerService.GetAreaData(data.id);

			ShowCraftingInformation(configurationData);
		}

		private void HandleOnCraftingAreaProcessSpeedUp(PlayerProgress_CraftingAreaData data)
		{
			_inProgressPanel.RefreshExpiration(data.expiration);
		}

		#endregion
	}
}