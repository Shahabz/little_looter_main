/*
 * Date: November 23th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_Player_NoAmmo : MonoBehaviour
    {
        [SerializeField] private GameObject _panel = default;
        [SerializeField] private WeaponController _weaponController = default;

		#region Unity events

		private void Awake()
		{
			_weaponController.OnRefreshAmmo += RefreshAmmo;
			_weaponController.OnStartReloading += StartReloading;
			_weaponController.OnStopReloading += StopReloading;
		}

		private void OnDestroy()
		{
			_weaponController.OnRefreshAmmo -= RefreshAmmo;
			_weaponController.OnStartReloading -= StartReloading;
			_weaponController.OnStopReloading -= StopReloading;
		}

		#endregion

		#region Private methods

		private void RefreshAmmo(int clipSize, int ammo)
		{
			if (clipSize > 0) return;

			ShowPanel();
		}

		private void StartReloading(float time)
		{
			// TODO: is it needed?
		}

		private void StopReloading()
		{
			HidePanel();
		}

		private void HidePanel()
		{
			_panel.SetActive(false);
		}

		private void ShowPanel()
		{
			_panel.SetActive(true);
		}

		#endregion
	}
}