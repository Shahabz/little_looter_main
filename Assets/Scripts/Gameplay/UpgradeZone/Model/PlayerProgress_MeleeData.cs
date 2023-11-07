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
        public bool toClaim;

        public void SetMeleeData(ConfigurationMeleeLevelData levelData)
		{
            damage = levelData.damage;
            level = levelData.level;

            PlayerProgressEvents.OnMeleeLevelUp?.Invoke();
		}

        public void StartUpgrade(float duration, float expiration)
		{
            isUpgrading = true;
            upgradeExpiration = expiration;

            var args = new PlayerProgressEvents.MeleeUpgradeStartedArgs()
            {
                duration = duration,
                expiration = expiration
            };

            PlayerProgressEvents.OnMeleeUpgradeStarted?.Invoke(args);
        }

        public void CompleteUpgrade()
		{
            isUpgrading = false;
            upgradeExpiration = 0;
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
    }
}