/*
 * Date: November 1st, 2023
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;
using System;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_PlayerResourcesPanel : MonoBehaviour
    {
        [SerializeField] private UI_PlayerResourcesPanel_Slot[] _slots = default;

		private void Awake()
		{
			UI_ResourcesAnimation.OnAnimationCompleted += HandleResourceAnimationCompleted;
			UI_GameplayEvents.OnConsumeResourcesByToolUpgrade += HandleConsumeResourcesByToolUpgrade;
			PlayerProgressEvents.OnSlotFixDone += HandleSlotFixed;
		}

		private void OnDestroy()
		{
			UI_ResourcesAnimation.OnAnimationCompleted -= HandleResourceAnimationCompleted;
			UI_GameplayEvents.OnConsumeResourcesByToolUpgrade -= HandleConsumeResourcesByToolUpgrade;
			PlayerProgressEvents.OnSlotFixDone -= HandleSlotFixed;
		}

		private void HandleSlotFixed(PlayerProgressEvents.RepairSlotArgs args)
		{
			RefreshSlot(args.resourceId);
		}

		private void HandleConsumeResourcesByToolUpgrade(MeleeUpgradeRequirementData[] requirements)
		{
			for (int i = 0; i < requirements.Length; i++)
			{
				var requirement = requirements[i];

				RefreshSlot(requirement.resource.Id);
			}
		}

		private void HandleResourceAnimationCompleted(int resourceId)
		{
			RefreshSlot(resourceId);
		}

		private void RefreshSlot(int resourceId)
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				var slot = _slots[i];

				if (slot.ResourceId != resourceId) continue;

				var resourceAmount = GetResourceAmount(resourceId);

				slot.Refresh(resourceAmount);

				break;
			}
		}

		private int GetResourceAmount(int id)
		{
			var service = ServiceLocator.Current.Get<PlayerProgressDataService>();
			var playerData = service.ProgressData;

			var resourceAmount = playerData.GetResourceAmount(id);

			return resourceAmount;
		}
	}
}