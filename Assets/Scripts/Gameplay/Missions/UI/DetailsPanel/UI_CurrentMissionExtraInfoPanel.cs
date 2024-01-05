/*
 * Date: November 18th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_CurrentMissionExtraInfoPanel : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private UI_CurrentMission_DestructionDetails _destructionPanel = default;
        [SerializeField] private UI_CurrentMission_UpgradeToolDetails _upgradeToolPanel = default;
		[SerializeField] private UI_CurrentMission_DeliveryDetails _deliveryPanel = default;
		[SerializeField] private UI_CurrentMission_RepairingDetails _repairingPanel = default;
		[SerializeField] private UI_CurrentMission_CraftingDetails _craftingPanel = default;

		#endregion

		#region Public methods

		public void Refresh(MissionConfigurationData mission)
		{
			if (mission.Type == MissionType.DESTRUCTION)
			{
				ShowDestructionPanel(mission);
				return;
			}

			if (mission.Type == MissionType.TOOL_UPGRADE)
			{
				ShowUpgradeToolPanel(mission);
				return;
			}

			if (mission.Type == MissionType.DELIVERY)
			{
				ShowDeliveryPanel(mission);
				return;
			}

			if (mission.Type == MissionType.REPAIRING)
			{
				ShowRepairingPanel(mission);
				return;
			}

			if (mission.Type == MissionType.CRAFTING)
			{
				ShowCraftingPanel(mission);
				return;
			}

			HidePanels();
		}

		#endregion

		#region Private methods

		private void HidePanels()
		{
			_destructionPanel.gameObject.SetActive(false);
			_upgradeToolPanel.gameObject.SetActive(false);
			_deliveryPanel.gameObject.SetActive(false);
			_repairingPanel.gameObject.SetActive(false);
			_craftingPanel.gameObject.SetActive(false);
		}

		private void ShowDestructionPanel(MissionConfigurationData mission)
		{
			HidePanels();

			_destructionPanel.gameObject.SetActive(true);

			var missionInfo = (MissionResourceDestructionData)mission;

			_destructionPanel.Setup(missionInfo.Destructible.LevelRequired);
		}

		private void ShowUpgradeToolPanel(MissionConfigurationData mission)
		{
			HidePanels();

			_upgradeToolPanel.gameObject.SetActive(true);

			var missionInfo = (MissionToolUpgradeData)mission;

			_upgradeToolPanel.Setup(missionInfo.ToolLevel);
		}

		private void ShowDeliveryPanel(MissionConfigurationData mission)
		{
			HidePanels();

			_deliveryPanel.gameObject.SetActive(true);

			var missionInfo = (MissionDeliveryData)mission;

			_deliveryPanel.Setup(missionInfo);
		}

		private void ShowRepairingPanel(MissionConfigurationData mission)
		{
			HidePanels();

			_repairingPanel.gameObject.SetActive(true);

			var missionInfo = (MissionRepairingData)mission;

			_repairingPanel.Setup(missionInfo.RepairObjectData);
		}

		private void ShowCraftingPanel(MissionConfigurationData mission)
		{
			HidePanels();

			_craftingPanel.gameObject.SetActive(true);

			var missionInfo = (MissionCraftingData)mission;

			_craftingPanel.Setup(missionInfo.AreaData.Id);
		}

		#endregion
	}
}