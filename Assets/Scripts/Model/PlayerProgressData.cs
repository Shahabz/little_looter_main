/*
 * Date: October 22th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay;
using System;
using System.Collections.Generic;

namespace LittleLooters.Model
{
    [Serializable]
    public struct PlayerProgressData
    {
        public PlayerProgress_ObjectToRepairData[] repairProgress;  // TODO: replace it for PlayerProgressRepairData
        public PlayerProgress_ResourcesData resourcesData;
        public PlayerProgress_MeleeData meleeData;

        public void SetupRepairObjects(Gameplay.RepairObject[] repairObjects)
		{
            var progress = new List<PlayerProgress_ObjectToRepairData>(repairObjects.Length);

			for (int i = 0; i < repairObjects.Length; i++)
			{
                var repairObjectData = repairObjects[i].Data;

                var repairObjectProgress = new PlayerProgress_ObjectToRepairData();

                repairObjectProgress.Setup(repairObjectData);

                progress.Add(repairObjectProgress);
			}

            repairProgress = progress.ToArray();
		}

        public (int, PlayerProgress_ObjectToRepairData) GetRepairObjectProgressData(int id)
		{
			for (int i = 0; i < repairProgress.Length; i++)
			{
                var progressData = repairProgress[i];

                if (progressData.id != id) continue;

                return (i, progressData);
			}

            UnityEngine.Debug.LogError($"<color=red>ERROR</color>: There is no Repair object for id <color=yellow>{id}</color>");

            return (0, new PlayerProgress_ObjectToRepairData());
		}

        public void AddPartsToRepairObject(int objectId, int partId, int partAmount)
		{
            UnityEngine.Debug.LogError($"Add part <color=cyan>{partId}</color> to object <color=yellow>{objectId}</color>");

            var (index, repairProgressData) = GetRepairObjectProgressData(objectId);

            repairProgressData.AddPartsTo(partId, partAmount);

            repairProgress[index] = repairProgressData;

            var allPartsRepaired = repairProgress[index].AllPartsCompleted();

            if (!allPartsRepaired) return;

            UnityEngine.Debug.LogError($"<color=yellow>{objectId}</color> COMPLETED!");

            var repairDuration = repairProgress[index].duration;

            repairProgress[index].expiration = UnityEngine.Time.time + repairDuration;
		}

        public void GrantResourceAmount(int id, int amount)
		{
            this.resourcesData.Grant(id, amount);
		}

		#region Melee methods

		public void SetMeleeData(ConfigurationMeleeLevelData levelData)
		{
            meleeData.SetMeleeData(levelData);
		}

        public void StartMeleeUpgrade(float duration, float expiration, MeleeUpgradeRequirementData[] requirements)
		{
			// Consume resources based on requirements
			for (int i = 0; i < requirements.Length; i++)
			{
                var requirement = requirements[i];

                var resourceId = requirement.resource.Id;
                var resourceAmount = requirement.amount;

                resourcesData.ConsumeResource(resourceId, resourceAmount);
			}

            // Start melee upgrading
            meleeData.StartUpgrade(duration, expiration);
		}

        public void CompleteMeleeUpgrade()
		{
            meleeData.CompleteUpgrade();
		}

        public void ClaimMeleeUpgrade(int newDamage)
		{
            meleeData.ClaimUpgrade(newDamage);
		}

		#endregion
	}
}