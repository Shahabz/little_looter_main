/*
 * Date: October 22th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay;
using LittleLooters.General;
using System;
using System.Collections.Generic;

namespace LittleLooters.Model
{
    [Serializable]
    public struct PlayerProgressData
    {
        public PlayerProgress_ObjectToRepairData[] repairProgress;  // TODO: replace it for PlayerProgressRepairData
        public PlayerProgress_ResourcesData resourcesData;
        public PlayerProgress_MeleeData toolData;
        public PlayerProgress_CraftingData craftingData;
        public PlayerProgress_MissionsData missionsData;

        public void InitRepairProgress(Gameplay.RepairObject[] repairObjects)
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

		/*public void AddPartsToRepairObject(int objectId, int partId, int partAmount)
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
		}*/

		#region Resource methods

        public void InitializeResources(ResourceData[] resources)
		{
            this.resourcesData.Initialize(resources);
		}

        public void GrantResourceAmount(int id, int amount)
		{
            this.resourcesData.Grant(id, amount);
		}

        public int GetResourceAmount(int id)
		{
            var resourceData = this.resourcesData.GetResourceAmount(id);

            return resourceData.amount;
		}

        public void ConsumeResourceAmount(int id, int amount)
		{
            this.resourcesData.ConsumeResource(id, amount);
        }

		#endregion

		#region Tool methods

		public void SetMeleeData(ConfigurationMeleeLevelData levelData)
		{
            toolData.SetMeleeData(levelData);
		}

        public void StartMeleeUpgrade(float duration, float expiration, MeleeUpgradeRequirementData[] requirements, int nextLevelDamage)
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
            toolData.StartUpgrade(duration, expiration, nextLevelDamage);
		}

        public void CompleteMeleeUpgrade()
		{
            toolData.CompleteUpgrade();
		}

        public void ClaimMeleeUpgrade(int newDamage)
		{
            toolData.ClaimUpgrade(newDamage);
		}

        public void SpeedUpToolUpgrade()
		{
            toolData.CompleteUpgrade();
        }

        public void IncreaseToolDamage(float now, int durationInSecs)
		{
            var expiration = now + durationInSecs;

            toolData.StartIncreaseDamage(expiration, durationInSecs);
		}

        public void CompleteIncreaseToolDamage(ConfigurationMeleeLevelData toolLevelData)
		{
            var realDamage = toolLevelData.damage;

            toolData.CompleteIncreaseDamage(realDamage);
		}

		#endregion

		#region Repair methods

        /// <summary>
        /// Returns the amount of this slot that was already fixed
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public int GetSlotRepairStatus(int objectId, int resourceId)
		{
            var (_, repairObject) = GetRepairObjectProgressData(objectId);

            var progress = repairObject.GetPartProgress(resourceId);

            return progress.amount;
        }

        public void Fix(int objectId, int resourceId)
		{
            var resourceAmount = GetResourceAmount(resourceId);
            var (index, repairObject) = GetRepairObjectProgressData(objectId);

            var consumedResourceAmount = repairObject.Fix(resourceId, resourceAmount);

            repairProgress[index] = repairObject;

            resourcesData.ConsumeResource(resourceId, consumedResourceAmount);

            var resourceProgress = repairObject.GetPartProgress(resourceId);

            var args = new PlayerProgressEvents.RepairSlotArgs()
            {
                objectId = objectId,
                resourceId = resourceId,
                currentAmount = resourceProgress.amount,
                totalAmount = resourceProgress.total,
                allSlotsDone = repairObject.AllPartsCompleted()
            };

            PlayerProgressEvents.OnSlotFixDone?.Invoke(args);
		}

        public void StartRepairing(int objectId, float startTimestamp)
		{
            var (index, repairObject) = GetRepairObjectProgressData(objectId);

            repairObject.StartRepairing(startTimestamp);

            repairProgress[index] = repairObject;

            var args = new PlayerProgressEvents.RepairStartActionArgs()
            {
                id = objectId,
                expiration = startTimestamp + repairObject.duration,
                duration = repairObject.duration
            };

            PlayerProgressEvents.OnStartRepairing?.Invoke(args);
        }

        public void CompleteRepairing(int objectId)
		{
            var (index, repairObject) = GetRepairObjectProgressData(objectId);

            repairObject.CompleteRepairing();

            repairProgress[index] = repairObject;

            PlayerProgressEvents.OnCompleteRepairing?.Invoke(objectId);
        }

        public void SpeedUpRepairing(int objectId)
		{
            var (index, repairObject) = GetRepairObjectProgressData(objectId);

            repairObject.SpeedUp();

            repairProgress[index] = repairObject;

            var args = new PlayerProgressEvents.RepairSpeedUpArgs()
            {
                id = objectId,
                expiration = repairObject.expiration,
                duration = repairObject.duration
            };

            if (repairObject.expiration > 0)
            {
                PlayerProgressEvents.OnSpeedUpRepairing?.Invoke(args);
                return;
            }

            CompleteRepairing(objectId);
        }

        #endregion

        #region Crafting methods

        public void InitCrafting()
        {
            craftingData.Initialize();
        }

        public void CraftingStartProcess(CraftingConfigurationData data, int amountToProduce)
		{
            // TODO: check if the crafting area is in status: NONE, else skip it

            // Consume resources required
            var amountToConsume = amountToProduce * data.AmountRequired;
            resourcesData.ConsumeResource(data.ResourceRequired.Id, amountToConsume);

            // Persist area progress
            craftingData.StartProcess(data.Id, data.DurationByUnitInSecs, amountToProduce, UnityEngine.Time.time);
		}

        public void CraftingCompleteProcess(int areaId)
		{
            craftingData.CompleteProcess(areaId);
		}

        public int CraftingClaimProcess(int areaId, int resourceId)
		{
            // Get produced resource amount 
            var areaProgressData = craftingData.GetAreaProgressData(areaId);

            var amountObtained = areaProgressData.amount;

            // Grant produced amount
            resourcesData.Grant(resourceId, areaProgressData.amount);

            // Claim area process
            craftingData.ClaimProcess(areaId);

            return amountObtained;
		}

        public void CraftingSpeedUpProcess(int areaId, int seconds, float now)
		{
            craftingData.SpeedUpProcess(areaId, seconds, now);
        }

        #endregion

        #region Missions

        public void UpdateCurrentMission(int id)
        {
            missionsData.UpdateCurrentMission(id);
        }

        #endregion
    }
}