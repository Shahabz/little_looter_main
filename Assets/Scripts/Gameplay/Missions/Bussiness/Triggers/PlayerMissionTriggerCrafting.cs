/*
 * Date: December 27th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using LittleLooters.Model;
using System;

namespace LittleLooters.Gameplay
{
    public class PlayerMissionTriggerCrafting
    {
		#region Private properties

		private const MissionType _missionType = MissionType.CRAFTING;
		private Action _callback = null;
		private PlayerCraftingService _craftingService = default;
		private PlayerEntryPoint _entryPoint = default;
		private int _amountGoal = 0;
		private int _amount = 0;
		private bool _inProgress = false;
		private ResourceData _resource = default;

		#endregion

		#region Public methods

		public void Initialize(Action callback, PlayerCraftingService service, PlayerEntryPoint entryPoint)
		{
			_callback = callback;

			_craftingService = service;

			_entryPoint = entryPoint;

			PlayerProgressEvents.OnCraftingAreaProcessClaimed += HandleCraftingAreaClaim;
		}

		public void Teardown()
		{
			_callback = null;

			PlayerProgressEvents.OnCraftingAreaProcessClaimed -= HandleCraftingAreaClaim;
		}

		public void ResetStatus(MissionType type, MissionConfigurationData mission)
		{
			if (_missionType != type)
			{
				Stop();

				return;
			}

			var data = (MissionCraftingData)mission;

			Start(data.Amount, data.ResourceData);
		}

		#endregion

		#region Private methods

		private void Start(int amount, ResourceData resourceData)
		{
			_resource = resourceData;
			_amountGoal = amount;
			_amount = 0;
			_inProgress = true;
		}

		private void Stop()
		{
			_resource = null;
			_amountGoal = 0;
			_amount = 0;
			_inProgress = false;
		}

		private void HandleCraftingAreaClaim(PlayerProgress_CraftingAreaData areaData)
		{
			var areaId = areaData.id;

			var configurationData = _craftingService.GetAreaData(areaId);

			var resourceId = configurationData.ResourceGenerated.Id;

			if (resourceId != _resource.Id) return;

			_amount = _entryPoint.ProgressData.GetResourceAmount(resourceId);

			PlayerMissionsEvents.OnMissionProgress?.Invoke(_amount, _amountGoal);

			if (_amount < _amountGoal) return;

			_callback?.Invoke();
		}

		#endregion

	}
}