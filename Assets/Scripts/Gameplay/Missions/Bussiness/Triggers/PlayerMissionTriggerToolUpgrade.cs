/*
 * Date: November 15th, 2023
 * Author: Peche
 */

using LittleLooters.Model;

namespace LittleLooters.Gameplay
{
	public class PlayerMissionTriggerToolUpgrade
	{
		private const MissionType _missionType = MissionType.TOOL_UPGRADE;
		private int _levelGoal = -1;
		private bool _inProgress = false;
		private System.Action _callback = null;

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
		}

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

			PlayerMissionsEvents.OnMissionProgress?.Invoke(1, 1);

			_callback?.Invoke();
		}

		private void ExplorableFound(ExplorableObjectType explorableType)
		{
			if (!_inProgress) return;

			if (explorableType != ExplorableObjectType.UPGRADE_TOOL) return;

			UI_GameplayEvents.OnStopMissionAssistance?.Invoke();
		}
	}
}