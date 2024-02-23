/*
 * Date: November 15th, 2023
 * Author: Peche
 */

using LittleLooters.Model;

namespace LittleLooters.Gameplay
{
	public class PlayerMissionTriggerToolUpgrade
	{
		#region Private properties

		private const MissionType _missionType = MissionType.TOOL_UPGRADE;
		private int _levelGoal = -1;
		private bool _inProgress = false;
		private System.Action _callback = null;

		#endregion

		#region Public methods

		public void Initialize(System.Action callback)
		{
			_callback = callback;

			PlayerProgressEvents.OnMeleeUpgradeClaimed += ToolUpgradeClaimed;

			ExplorableObjectEvents.OnEnter += ExplorableFound;
		}

		public void Teardown()
		{
			PlayerProgressEvents.OnMeleeUpgradeClaimed -= ToolUpgradeClaimed;

			ExplorableObjectEvents.OnEnter -= ExplorableFound;

			_callback = null;
		}

		public void ResetStatus(MissionType type, MissionConfigurationData mission)
		{
			if (_missionType != type)
			{
				Stop();
				return;
			}

			var data = (MissionToolUpgradeData) mission;

			Start(data.ToolLevel);

			CheckAlreadyCompleted(data);
		}

		#endregion

		#region Private methods

		private void Start(int level)
		{
			_levelGoal = level;
			_inProgress = true;
		}

		private void Stop()
		{
			_levelGoal = -1;
			_inProgress = false;
		}

		private void ToolUpgradeClaimed(PlayerProgressEvents.MeleeUpgradeClaimedArgs args)
		{
			if (!_inProgress) return;

			if (args.level != _levelGoal) return;

			MarkAsCompleted();
		}

		private void MarkAsCompleted()
		{
			PlayerMissionsEvents.OnMissionProgress?.Invoke(1, 1);

			_callback?.Invoke();
		}

		private void ExplorableFound(ExplorableObjectType explorableType, int objectId)
		{
			if (!_inProgress) return;

			if (explorableType != ExplorableObjectType.UPGRADE_TOOL) return;

			UI_GameplayEvents.OnStopMissionAssistance?.Invoke();
		}

		private async void CheckAlreadyCompleted(MissionToolUpgradeData data)
		{
			// Get delay time
			var gameConfigurationService = Global.ServiceLocator.ServiceLocator.Current.Get<Global.ServiceLocator.GameConfigurationService>();

			var delay = gameConfigurationService.TimeToCheckCompletedMission;

			// Wait delay
			await System.Threading.Tasks.Task.Delay(UnityEngine.Mathf.CeilToInt(1000 * delay));
			
			// Get level goal
			var level = data.ToolLevel;

			// Get current tool level
			var playerProgressDataService = Global.ServiceLocator.ServiceLocator.Current.Get<Global.ServiceLocator.PlayerProgressDataService>();

			var progressData = playerProgressDataService.ProgressData;

			// Compare current tool level with goal level
			var currentToolLevel = progressData.toolData.level;

			if (currentToolLevel < level) return;

			MarkAsCompleted();
		}

		#endregion
	}
}