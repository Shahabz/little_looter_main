/*
 * Date: November 18th, 2023
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_CurrentMission_DestructionDetails : MonoBehaviour
    {
		#region Inspector

        [SerializeField] private TMPro.TextMeshProUGUI _txtLevel = default;
        [SerializeField] private Image _imgCheck = default;
		[SerializeField] private Color _colorLevelNotReached = default;

		#endregion

		#region Private properties

		private int _toolLevelRequirement = 0;

		#endregion

		#region Unity events

		private void OnEnable()
		{
			UI_GameplayEvents.OnStartGame += HandleStartGame;

            PlayerProgressEvents.OnMeleeUpgradeClaimed += HandleToolUpgradeClaimed;
		}

        private void OnDisable()
        {
			UI_GameplayEvents.OnStartGame -= HandleStartGame;

			PlayerProgressEvents.OnMeleeUpgradeClaimed -= HandleToolUpgradeClaimed;
        }

		#endregion

		#region Public methods

		public void Setup(int toolLevel)
		{
            _toolLevelRequirement = toolLevel;

            CheckToolLevel();

            _txtLevel.text = $"TOOL LEVEL {toolLevel}";   // TODO: localize
		}

		#endregion

		#region Private methods

		private void HandleStartGame()
		{
			CheckToolLevel();
		}

		private void HandleToolUpgradeClaimed(PlayerProgressEvents.MeleeUpgradeClaimedArgs args)
        {
            CheckToolLevel();
        }

        private void CheckToolLevel()
		{
			var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();

			var currentToolLevel = progressDataService.ProgressData.toolData.level;

			var goalReached = currentToolLevel >= _toolLevelRequirement;

			_imgCheck.enabled = goalReached;

			_txtLevel.color = (goalReached) ? Color.white : _colorLevelNotReached;
		}

        #endregion
    }
}