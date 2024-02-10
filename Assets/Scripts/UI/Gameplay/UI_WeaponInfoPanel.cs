/*
 * Date: September 2nd, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using UnityEngine;
using UnityEngine.UI;

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
		[SerializeField] private int _fontSize = 60;
		[SerializeField] private GameObject _reloading = default;
		[SerializeField] private Slider _progress = default;
		[SerializeField] private Slider _fireRateProgress = default;
		[SerializeField] private Image _fireRateFill = default;
		[SerializeField] private Color _fireRateColorFull = default;
		[SerializeField] private Color _fireRateColorCharging = default;
		[SerializeField] private Color _fireRateColorBad = default;

		#endregion

		#region Private properties

		private float _remainingReloadTime = 0;
		private float _totalReloadingTime = 0;
		private float _remainingFireRateTime = 0;
		private float _totalFireRateTime = 0;
		private float _stepTime = 0.10f;

		#endregion

		#region Unity events

		private void Awake()
		{
			UI_GameplayEvents.OnPlayerInitialization += Initialization;
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnPlayerInitialization -= Initialization;

			_weaponController.OnRefreshAmmo -= Refresh;
			_weaponController.OnStartReloading -= StartReloading;
			_weaponController.OnStopReloading -= StopReloading;

			// Firing
			_weaponController.OnStartFiring -= StartFiring;
			_weaponController.OnCompleteFiring -= CompleteFiring;
		}

		#endregion

		#region Private methods

		private void Initialization()
		{
			// Ammo
			_weaponController.OnRefreshAmmo += Refresh;

			// Reloading
			_weaponController.OnStartReloading += StartReloading;
			_weaponController.OnStopReloading += StopReloading;

			// Firing
			_weaponController.OnStartFiring += StartFiring;
			_weaponController.OnCompleteFiring += CompleteFiring;

			Refresh(_weaponController.ClipSize, _weaponController.Ammo);
		}

		private void Refresh(int clipSize, int ammo)
		{
			_txt.text = $"<size={_fontSize}>{clipSize}</size><color=white>/{ammo}</color>";
		}

		private void StartReloading(float reloadingTime)
		{
			/*
			_reloading.SetActive(true);

			_remainingReloadTime = 0;
			_totalReloadingTime = reloadingTime;

			InvokeRepeating(nameof(RefreshProgress), 0f, _stepTime);
			*/
		}

		private void StopReloading()
		{
			/*
			_reloading.SetActive(false);

			CancelInvoke(nameof(RefreshProgress));
			*/
		}

		private void RefreshProgress()
		{
			_remainingReloadTime += _stepTime;

			var progress = _remainingReloadTime / _totalReloadingTime;

			_progress.value = progress;
		}

		private void StartFiring(float fireRateTime)
		{
			_remainingFireRateTime = 0;
			_totalFireRateTime = fireRateTime;

			InvokeRepeating(nameof(RefreshFireRateProgress), 0f, _stepTime);
		}

		private void CompleteFiring()
		{
			CancelInvoke(nameof(RefreshFireRateProgress));
		}

		private void RefreshFireRateProgress()
		{
			_remainingFireRateTime += _stepTime;

			var progress = (_remainingFireRateTime / _totalFireRateTime);

			_fireRateProgress.value = progress;

			_fireRateFill.color = FireRateColorByProgress(progress);
		}

		private Color FireRateColorByProgress(float progress)
		{
			if (progress <= 0.5f) return _fireRateColorBad;

			if (progress < 1) return _fireRateColorCharging;

			return _fireRateColorFull;
		}

		#endregion
	}
}