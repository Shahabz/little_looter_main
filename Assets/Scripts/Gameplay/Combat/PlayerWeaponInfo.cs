/*
 * Date: February 10th, 2024
 * Author: Peche
 */

using LittleLooters.Model;

namespace LittleLooters.Gameplay.Combat
{
    /// <summary>
    /// Represents the information related with a player's weapon 
    /// and its state like if it is reloading, remaining ammo, etc
    /// </summary>
    [System.Serializable]
    public class PlayerWeaponInfo
    {
		#region Public properties

		public string id;
        public int ammo;
        public bool isReloading;
        public float reloadingExpiration;
        public bool isSelected;
        public bool isLocked;

		#endregion

		#region Private properties

		private bool ammoChangeArgsInitialized;
        private PlayerProgressEvents.WeaponAmmoChangeArgs ammoChangeArgs;
        private bool startReloadingArgsInitialized;
        private PlayerProgressEvents.WeaponStartReloadingArgs startReloadingArgs;
        private bool stopReloadingArgsInitialized;
        private PlayerProgressEvents.WeaponStopReloadingArgs stopReloadingArgs;
        private bool selectionArgsInitialized;
        private PlayerProgressEvents.WeaponSelectionArgs selectionArgs;

        #endregion

        #region Public methods

        public void Init(int ammo, bool selected)
		{
            RefreshSelection(selected);

            RefreshAmmo(ammo);
		}

        public void ConsumeAmmo()
		{
            if (ammo == 0) return;

            ammo--;

            RefreshAmmo(ammo);
		}

        public void RefreshSelection(bool value)
		{
            isSelected = value;

            if (!selectionArgsInitialized)
			{
                selectionArgs = new PlayerProgressEvents.WeaponSelectionArgs();
                selectionArgsInitialized = true;
			}

            selectionArgs.id = id;
            selectionArgs.isSelected = value;

            PlayerProgressEvents.OnWeaponSelectionChanged?.Invoke(selectionArgs);
		}

        public void StartReloading(float expiration, float duration)
		{
            isReloading = true;
            reloadingExpiration = expiration;

            if (!startReloadingArgsInitialized)
			{
                startReloadingArgs = new PlayerProgressEvents.WeaponStartReloadingArgs();
                startReloadingArgsInitialized = true;
			}

            startReloadingArgs.id = id;
            startReloadingArgs.expiration = reloadingExpiration;
            startReloadingArgs.duration = duration;

            if (canDebug) Debug_StartReloading();

            PlayerProgressEvents.OnWeaponStartReloading?.Invoke(startReloadingArgs);
		}

        public void StopReloading(int amount)
		{
            isReloading = false;
            reloadingExpiration = 0;

            if (!stopReloadingArgsInitialized)
			{
                stopReloadingArgs = new PlayerProgressEvents.WeaponStopReloadingArgs();
                stopReloadingArgsInitialized = true;
			}

            stopReloadingArgs.id = id;

            PlayerProgressEvents.OnWeaponStopReloading?.Invoke(stopReloadingArgs);

            if (canDebug) Debug_StopReloading(amount);

            RefreshAmmo(amount);
		}

        #endregion

        #region Private methods

        private void RefreshAmmo(int amount)
        {
            ammo = amount;

            if (!ammoChangeArgsInitialized)
            {
                ammoChangeArgs = new PlayerProgressEvents.WeaponAmmoChangeArgs();
                ammoChangeArgsInitialized = true;
            }

            ammoChangeArgs.id = id;
            ammoChangeArgs.ammo = ammo;

            if (canDebug) Debug_RefreshAmmo();

            PlayerProgressEvents.OnWeaponAmmoChanged?.Invoke(ammoChangeArgs);
        }

        #endregion

        #region Debug

        private bool canDebug = false;

        private void Debug_StartReloading()
		{
            UnityEngine.Debug.LogError($"Weapon '<color=yellow>{id}</color>' start reloading");
        }

        private void Debug_StopReloading(int amount)
        {
            UnityEngine.Debug.LogError($"Weapon '<color=yellow>{id}</color>' stop reloading, ammo: <color=magenta>{amount}</color>");
        }

        private void Debug_RefreshAmmo()
        {
            UnityEngine.Debug.LogError($"Weapon '<color=yellow>{id}</color>' refrehs ammo: <color=magenta>{ammo}</color>");
        }

        #endregion
    }
}