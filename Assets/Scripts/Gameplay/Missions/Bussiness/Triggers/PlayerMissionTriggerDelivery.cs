/*
 * Date: December 28th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using LittleLooters.Model;
using System;

namespace LittleLooters.Gameplay
{
    public class PlayerMissionTriggerDelivery
    {
		#region Private properties

		private const MissionType _missionType = MissionType.DELIVERY;
		private Action _callback = null;
		private int _amountGoal = 0;
		private int _amount = 0;
		private bool _inProgress = false;
		private ResourceData _resourceData = default;
		private RepairObjectData _repairObjectData = default;
		private PlayerEntryPoint _entryPoint = default;

		#endregion

		#region Public methods

		public void Initialize(Action callback, PlayerEntryPoint entryPoint)
		{
			_callback = callback;

			_entryPoint = entryPoint;

			PlayerProgressEvents.OnSlotFixDone += HandleOnSlotFixDone;
		}

		public void Teardown()
		{
			_callback = null;

			PlayerProgressEvents.OnSlotFixDone -= HandleOnSlotFixDone;
		}

		public void ResetStatus(MissionType type, MissionConfigurationData mission)
		{
			if (_missionType != type)
			{
				Stop();

				return;
			}

			var data = (MissionDeliveryData)mission;

			Start(data);
		}

		#endregion

		#region Private methods

		private void Start(MissionDeliveryData data)
		{
			_repairObjectData = data.RepairObjectData;
			_resourceData = data.ResourceData;
			_amountGoal = data.Amount;
			_amount = _entryPoint.ProgressData.GetSlotRepairStatus(_repairObjectData.Id, _resourceData.Id);
			_inProgress = true;
		}

		private void Stop()
		{
			_repairObjectData = null;
			_resourceData = null;
			_amountGoal = 0;
			_amount = 0;
			_inProgress = false;
		}

		private void HandleOnSlotFixDone(PlayerProgressEvents.RepairSlotArgs args)
		{
			var repairObjectId = args.objectId;

			// Check if it is the repair object goal
			if (_repairObjectData.Id != repairObjectId) return;

			var resourceId = args.resourceId;

			// Check if it is the resource goal
			if (resourceId != _resourceData.Id) return;

			_amount = args.currentAmount;

			PlayerMissionsEvents.OnMissionProgress?.Invoke(_amount, _amountGoal);

			if (_amount < _amountGoal) return;

			_callback?.Invoke();
		}

		#endregion
	}
}