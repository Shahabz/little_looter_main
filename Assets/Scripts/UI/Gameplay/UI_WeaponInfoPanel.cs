/*
 * Date: September 2nd, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
	/// <summary>
	/// In charge of updating the clip size and remaining ammo that player has.
	/// It is refreshed after:
	///   - Firing
	///   - Reloading
	/// </summary>
    public class UI_WeaponInfoPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private WeaponController _weaponController = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txt = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtReloading = default;

		#endregion

		#region Unity events

		private void Start()
		{
			_weaponController.OnRefreshAmmo += Refresh;
			_weaponController.OnStartReloading += StartReloading;
			_weaponController.OnStopReloading += StopReloading;
		}

		private void OnDestroy()
		{
			_weaponController.OnRefreshAmmo -= Refresh;
			_weaponController.OnStartReloading -= StartReloading;
			_weaponController.OnStopReloading -= StopReloading;
		}

		#endregion

		#region Private methods

		private void Refresh(int clipSize, int ammo)
		{
			_txt.text = $"<color=white>{clipSize}/</color><size=30>{ammo}</size>";
		}

		private void StartReloading()
		{
			_txtReloading.enabled = true;
		}

		private void StopReloading()
		{
			_txtReloading.enabled = false;
		}

		#endregion
	}
}