/*
 * Date: November 15th, 2023
 * Author: Peche
 */

using System.Threading.Tasks;

namespace LittleLooters.Gameplay
{
	public class PlayerMissionTriggerExploration
	{
		#region Private properties

		private const MissionType _missionType = MissionType.EXPLORATION;
		private ExplorableObjectType _goal = ExplorableObjectType.NONE;
		private bool _inProgress = false;
		private System.Action _callback = null;
		private int _objectId = -1;

		#endregion

		#region Public methods

		public void Initialize(System.Action callback)
		{
			_callback = callback;

			ExplorableObjectEvents.OnEnter += ExplorableFound;
		}

		public void Teardown()
		{
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

			var data = (MissionExplorationData) mission;

			Start(data.explorableType, data.Id);

			CheckAlreadyCompleted(data);
		}

		#endregion

		#region Private methods

		private void Start(ExplorableObjectType type, int id)
		{
			_goal = type;
			_inProgress = true;
			_objectId = id;
		}

		private void Stop()
		{
			_goal = ExplorableObjectType.NONE;
			_inProgress = false;
			_objectId = -1;
		}

		private void ExplorableFound(ExplorableObjectType explorableType, int id)
		{
			if (!_inProgress) return;

			if (explorableType != _goal) return;

			if (_objectId != id) return;

			MarkAsCompleted();
		}

		private void MarkAsCompleted()
		{
			UI_GameplayEvents.OnStopMissionAssistance?.Invoke();

			PlayerMissionsEvents.OnMissionProgress?.Invoke(1, 1);

			_callback?.Invoke();
		}

		private async void CheckAlreadyCompleted(MissionExplorationData data)
		{
			if (data.explorableType != ExplorableObjectType.RESOURCE) return;

			var gameConfigurationService = Global.ServiceLocator.ServiceLocator.Current.Get<Global.ServiceLocator.GameConfigurationService>();

			var delay = gameConfigurationService.TimeToCheckCompletedMission;

			await Task.Delay(UnityEngine.Mathf.CeilToInt(1000 * delay));

			// Check if resource is part of player's resources
			var playerProgressDataService = Global.ServiceLocator.ServiceLocator.Current.Get<Global.ServiceLocator.PlayerProgressDataService>();

			var progressData = playerProgressDataService.ProgressData;

			var resourceAmount = progressData.GetResourceAmount(data.Id);

			if (resourceAmount > 0)
			{
				MarkAsCompleted();
				return;
			}

			// Check if resource was delivered on some repairable object
			var wasDelivered = progressData.ResourceWasDelivered(data.Id);

			if (!wasDelivered) return;

			MarkAsCompleted();
		}

		#endregion
	}
}