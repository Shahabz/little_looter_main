/*
 * Date: November 5th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay;
using System;

namespace LittleLooters.Model
{
    [Serializable]
    public struct PlayerProgress_MeleeData
    {
        public int level;
        public int damage;
        public bool isUpgrading;
        public float upgradeExpiration;
        public float upgradeDuration;
        public bool toClaim;
        public float extraDamageExpiration;
        public bool extraDamageActive;

        public void SetMeleeData(ConfigurationMeleeLevelData levelData)
		{
            damage = levelData.damage;
            level = levelData.level;

            PlayerProgressEvents.OnMeleeLevelUp?.Invoke();
		}

        public void StartUpgrade(float duration, float expiration, int nextLevelDamage)
		{
            isUpgrading = true;
            upgradeExpiration = expiration;
            upgradeDuration = duration;

            var args = new PlayerProgressEvents.MeleeUpgradeStartedArgs()
            {
                duration = duration,
                expiration = expiration,
                currentDamage = damage,
                nextLevelDamage = nextLevelDamage
            };

            PlayerProgressEvents.OnMeleeUpgradeStarted?.Invoke(args);
        }

        public void CompleteUpgrade()
		{
            isUpgrading = false;
            upgradeExpiration = 0;
            upgradeDuration = 0;
            toClaim = true;

            PlayerProgressEvents.OnMeleeUpgradeCompleted?.Invoke();
        }

        public void ClaimUpgrade(int newDamage)
		{
            var args = new PlayerProgressEvents.MeleeUpgradeClaimedArgs()
            {
                level = level+1,
                oldDamage = damage,
                newDamage = newDamage
            };

            toClaim = false;
            level++;
            damage = newDamage;

            PlayerProgressEvents.OnMeleeUpgradeClaimed?.Invoke(args);
        }

        public void StartIncreaseDamage(float expiration, int duration)
		{
            if (extraDamageActive) return;

            damage = damage * 3;

            extraDamageActive = true;
            extraDamageExpiration = expiration;

            var args = new PlayerProgressEvents.ToolExtraDamageStartedArgs()
            {
                expiration = expiration,
                duration = duration
            };

            PlayerProgressEvents.OnToolDamageIncreaseStarted?.Invoke(args);
		}

        public void CompleteIncreaseDamage(int realDamage)
		{
            if (!extraDamageActive) return;

            damage = realDamage;
            extraDamageActive = false;
            extraDamageExpiration = 0;

            PlayerProgressEvents.OnToolDamageIncreaseCompleted?.Invoke();
		}

        public void SpeedUpUpgradeByWatchingAd()
		{
            var secondsToSkip = Constants.SKIP_TIME_SECS;
            var now = UnityEngine.Time.time;
            var newExpiration = upgradeExpiration - secondsToSkip;

            // Check upgrade completion
            if (newExpiration <= now)
			{
                CompleteUpgrade();
                return;
			}

            upgradeExpiration = newExpiration;

            var args = new PlayerProgressEvents.ToolUpgradeExpirationChangedArgs()
            {
                duration = upgradeDuration,
                expiration = upgradeExpiration
            };

            PlayerProgressEvents.OnToolUpgradeExpirationHasChanged?.Invoke(args);
        }

        /// <summary>
        /// Checks if player's resources are enough to upgrade the player's tool to the next level
        /// </summary>
        /// <param name="playerResources"></param>
        /// <param name="toolNextLevelData"></param>
        /// <returns></returns>
        public bool CheckUpgradeRequirementsReady(PlayerProgress_ResourcesData playerResources, ConfigurationMeleeLevelData toolNextLevelData)
		{
            var canUpgrade = true;

            for (int i = 0; i < toolNextLevelData.requirements.Length; i++)
            {
                var requirement = toolNextLevelData.requirements[i];

                var playerResource = playerResources.GetResourceAmount(requirement.resource.Id);

                if (playerResource.amount < requirement.amount)
                {
                    canUpgrade = false;
                    break;
                }
            }

            return canUpgrade;
		}
    }
}