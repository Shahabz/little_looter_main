/*
 * Date: November 23th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using LittleLooters.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_Player_Reloading : MonoBehaviour
    {
        [SerializeField] private GameObject _panel = default;
        [SerializeField] private WeaponController _weaponController = default;
        [SerializeField] private Image _progressBarFill = default;

		#region Private properties

		private string _weaponId = default;
		private float _remainingReloadTime = 0;
		private float _totalReloadingTime = 0;
		private float _stepTime = 0.10f;

		#endregion

		#region Unity events

		private void Awake()
		{
			//_weaponController.OnStartReloading += StartReloading;
			//_weaponController.OnStopReloading += StopReloading;

			UI_GameplayEvents.OnStartGame += HandleStartGame;

			PlayerProgressEvents.OnWeaponSelectionChanged += HandleWeaponSelection;
			PlayerProgressEvents.OnWeaponStartReloading += HandleWeaponStartReloading;
			PlayerProgressEvents.OnWeaponStopReloading += HandleWeaponStopReloading;
		}

		private void OnDestroy()
		{
			//_weaponController.OnStartReloading -= StartReloading;
			//_weaponController.OnStopReloading -= StopReloading;

			UI_GameplayEvents.OnStartGame -= HandleStartGame;

			PlayerProgressEvents.OnWeaponSelectionChanged -= HandleWeaponSelection;
			PlayerProgressEvents.OnWeaponStartReloading -= HandleWeaponStartReloading;
			PlayerProgressEvents.OnWeaponStopReloading -= HandleWeaponStopReloading;
		}

		#endregion

		#region Private methods
		
		private void HandleStartGame()
		{
			_weaponId = _weaponController.CurrentWeaponId;
		}

		private void StartReloading(float reloadingTime)
		{
			ShowPanel();

			_remainingReloadTime = 0;
			_totalReloadingTime = reloadingTime;

			InvokeRepeating(nameof(RefreshProgress), 0f, _stepTime);
		}

		private void StopReloading()
		{
			CancelInvoke(nameof(RefreshProgress));

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

		private void RefreshProgress()
		{
			_remainingReloadTime += _stepTime;

			var progress = _remainingReloadTime / _totalReloadingTime;

			_progressBarFill.fillAmount = progress;
		}

		private void HandleWeaponSelection(PlayerProgressEvents.WeaponSelectionArgs args)
		{
			if (!args.isSelected) return;

			_weaponId = args.id;

			StopReloading();
		}

		private void HandleWeaponStartReloading(PlayerProgressEvents.WeaponStartReloadingArgs args)
		{
			if (!_weaponId.Equals(args.id)) return;

			StartReloading(args.duration);
		}

		private void HandleWeaponStopReloading(PlayerProgressEvents.WeaponStopReloadingArgs args)
		{
			StopReloading();
		}

		#endregion
	}
}