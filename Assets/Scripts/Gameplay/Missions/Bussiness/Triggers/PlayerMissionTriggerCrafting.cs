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
		private int _areaId = -1;

		#endregion

		#region Public methods

		public void Initialize(Action callback, PlayerCraftingService service, PlayerEntryPoint entryPoint)
		{
			_callback = callback;

			_craftingService = service;

			_entryPoint = entryPoint;

			PlayerProgressEvents.OnCraftingAreaProcessClaimed += HandleCraftingAreaClaim;
			ExplorableObjectEvents.OnEnter += HandleExplorableFound;
		}

		public void Teardown()
		{
			_callback = null;

			PlayerProgressEvents.OnCraftingAreaProcessClaimed -= HandleCraftingAreaClaim;
			ExplorableObjectEvents.OnEnter -= HandleExplorableFound;
		}

		public void ResetStatus(MissionType type, MissionConfigurationData mission)
		{
			if (_missionType != type)
			{
				Stop();

				return;
			}

			var data = (MissionCraftingData)mission;

			Start(data.Amount, data.ResourceData, data.AreaData.Id);
		}

		#endregion

		#region Private methods

		private void Start(int amount, ResourceData resourceData, int areaId)
		{
			_areaId = areaId;
			_resource = resourceData;
			_amountGoal = amount;
			_amount = 0;
			_inProgress = true;
		}

		private void Stop()
		{
			_areaId = -1;
			_resource = null;
			_amountGoal = 0;
			_amount = 0;
			_inProgress = false;
		}

		private void HandleCraftingAreaClaim(PlayerProgress_CraftingAreaData areaData)
		{
			if (!_inProgress) return;

			var areaId = areaData.id;

			var configurationData = _craftingService.GetConfigurationAreaData(areaId);

			var resourceId = configurationData.ResourceGenerated.Id;

			if (resourceId != _resource.Id) return;

			_amount += areaData.amount;

			PlayerMissionsEvents.OnMissionProgress?.Invoke(_amount, _amountGoal);

			if (_canDebug) DebugClaiming();

			if (_amount < _amountGoal) return;

			_callback?.Invoke();
		}

		private void HandleExplorableFound(ExplorableObjectType type, int id)
		{
			if (!_inProgress) return;

			if (type != ExplorableObjectType.CRAFTING_AREA) return;

			if (_areaId != id) return;

			UI_GameplayEvents.OnStopMissionAssistance?.Invoke();
		}

		#endregion

		#region Debug

		private bool _canDebug = false;

		private void DebugClaiming()
		{
			UnityEngine.Debug.LogError($"<color=orange>PlayerMissionTriggerCrafting</color>::HandleCraftingAreaClaim -> amount: {_amount}, goal: {_amountGoal}");
		}

		#endregion
	}
}