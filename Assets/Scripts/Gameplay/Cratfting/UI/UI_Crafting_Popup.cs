/*
 * Date: December 16th, 2023
 * Author: Peche
 */

using LittleLooters.General;
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
        [SerializeField] private UI_Crafting_InformationPanel _infoPanel = default;
        [SerializeField] private UI_Crafting_InProgressPanel _inProgressPanel = default;
        [SerializeField] private UI_Crafting_ClaimPanel _claimPanel = default;

		#endregion

		#region Unity events

		private void OnEnable()
		{
            PlayerCraftingEvents.OnStartAreaInteraction += HandleStartAreaInteraction;
            PlayerCraftingEvents.OnStopAreaInteraction += HandleStopAreaInteraction;
        }

		private void Start()
		{
			HideAllPanels();
		}

		private void OnDisable()
        {
            PlayerCraftingEvents.OnStartAreaInteraction -= HandleStartAreaInteraction;
            PlayerCraftingEvents.OnStopAreaInteraction -= HandleStopAreaInteraction;
        }

		#endregion

		#region Private methods

		private void HandleStartAreaInteraction(CraftingConfigurationData data)
		{
			var craftingStatus = _entryPoint.ProgressData.craftingData.GetAreaStatus(data.Id);

			if (craftingStatus == Model.CraftingStatus.NONE)
			{
				ShowCraftingInformation(data);
				return;
			}

			// TODO: check status IN PROGRESS

			// TODO: check status COMPLETED
		}

		private void HideAllPanels()
		{
			_infoPanel.Hide();
			_inProgressPanel.Hide();
			_claimPanel.Hide();
		}

		private void HandleStopAreaInteraction()
		{
			HideAllPanels();
		}

		private void ShowCraftingInformation(CraftingConfigurationData data)
		{
			_infoPanel.Show(data, _entryPoint.ProgressData);

			_inProgressPanel.Hide();
			_claimPanel.Hide();
		}

		#endregion
	}
}