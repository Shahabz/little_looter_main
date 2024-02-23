/*
 * Date: December 28th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;
using System;

namespace LittleLooters.Gameplay
{
    public class PlayerMissionTriggerRepairing
    {
		#region Private properties

		private const MissionType _missionType = MissionType.REPAIRING;
		private Action _callback = null;
		private int _repairObjectId = -1;
		private bool _inProgress = false;

		#endregion

		#region Public methods

		public void Initialize(Action callback)
		{
			_callback = callback;

			// Subscribe events
			PlayerProgressEvents.OnCompleteRepairing += HandleRepairingCompleted;
			ExplorableObjectEvents.OnEnter += HandleOnExplorableFound;
		}

		public void Teardown()
		{
			_callback = null;

			// Unsubscribe events
			PlayerProgressEvents.OnCompleteRepairing -= HandleRepairingCompleted;
			ExplorableObjectEvents.OnEnter -= HandleOnExplorableFound;
		}

		public void ResetStatus(MissionType type, MissionConfigurationData mission)
		{
			if (_missionType != type)
			{
				Stop();

				return;
			}

			var missionInfo = (MissionRepairingData)mission;
			
			Start(missionInfo);

			CheckAlreadyCompleted(missionInfo);
		}

		#endregion

		#region Private methods

		private void Start(MissionRepairingData data)
		{
			_repairObjectId = data.RepairObjectData.Id;
			_inProgress = true;
		}

		private void Stop()
		{
			_repairObjectId = -1;
			_inProgress = false;
		}

		private void HandleRepairingCompleted(int repairObjectId)
		{
			if (!_inProgress) return;

			if (_repairObjectId != repairObjectId) return;

			MarkAsCompleted();
		}

		private void MarkAsCompleted()
		{
			PlayerMissionsEvents.OnMissionProgress?.Invoke(1, 1);

			_callback?.Invoke();
		}

		private void HandleOnExplorableFound(ExplorableObjectType objectType, int objectId)
		{
			if (!_inProgress) return;

			if (objectType != ExplorableObjectType.REPAIRABLE_OBJECT) return;

			if (_repairObjectId != objectId) return;

			UI_GameplayEvents.OnStopMissionAssistance?.Invoke();
		}

		private async void CheckAlreadyCompleted(MissionRepairingData data)
		{
			// Get delay time
			var gameConfigurationService = ServiceLocator.Current.Get<GameConfigurationService>();

			var delay = gameConfigurationService.TimeToCheckCompletedMission;

			// Wait delay
			await System.Threading.Tasks.Task.Delay(UnityEngine.Mathf.CeilToInt(1000 * delay));

			// Get player progress data
			var playerProgressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();

			var progressData = playerProgressDataService.ProgressData;

			// Check object was already repaired
			var wasRepaired = progressData.RepairableObjectWasFixed(data.RepairObjectData.Id);

			if (!wasRepaired) return;

			MarkAsCompleted();
		}

		#endregion
	}
}