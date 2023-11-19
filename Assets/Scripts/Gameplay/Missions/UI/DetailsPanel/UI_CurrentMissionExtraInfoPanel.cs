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
				ShowUpgradeToolPanel();
				return;
			}
		}

		#endregion

		#region Private methods

		private void HidePanels()
		{
			_destructionPanel.gameObject.SetActive(false);
			_upgradeToolPanel.gameObject.SetActive(false);
		}

		private void ShowDestructionPanel(MissionConfigurationData mission)
		{
			HidePanels();

			// NOTE: there is no need to show anything yet
			//_destructionPanel.gameObject.SetActive(true);
			//_destructionPanel.Setup(mission);
		}

		private void ShowUpgradeToolPanel()
		{
			HidePanels();

			_upgradeToolPanel.gameObject.SetActive(true);
			_upgradeToolPanel.Setup();
		}

		#endregion
	}
}