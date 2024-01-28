/*
 * Date: September 30th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_WeaponSelectionSlot : MonoBehaviour
    {
		[SerializeField] private TMPro.TextMeshProUGUI _txtAmmo = default;
		[SerializeField] private Button _btn = default;
		[SerializeField] private Image _border = default;
		[SerializeField] private Sprite _borderSelected = default;
		[SerializeField] private Sprite _borderNonSelected = default;

		private int _id = -1;
		private bool _isSelected = false;
		private System.Action<int> _callback = default;

		public int Id => _id;

        public void Init(int id, bool isSelected, System.Action<int> callback, WeaponController weaponController)
		{
			_id = id;

			_callback = callback;

			_btn.onClick.AddListener(Select);

			RefreshAmmo(weaponController.ClipSize);

			weaponController.OnRefreshAmmo += HandleRefreshAmmo;
			weaponController.OnStartReloading += HandleStartReloading;
			weaponController.OnStopReloading += HandleStopReloading;

			if (isSelected)
			{
				MarkAsSelected();
				return;
			}

			MarkAsNonSelected();
		}

        public void Teardown(WeaponController weaponController)
		{
			_callback = null;

			weaponController.OnRefreshAmmo -= HandleRefreshAmmo;
			weaponController.OnStartReloading -= HandleStartReloading;
			weaponController.OnStopReloading -= HandleStopReloading;

			_btn.onClick.RemoveAllListeners();
		}

        public void MarkAsSelected()
		{
			_isSelected = true;

			_border.sprite = _borderSelected;

			var position = transform.localPosition;
			position.y = 20;

			transform.localPosition = position;
		}

		public void MarkAsNonSelected()
		{
			_isSelected = false;

			_border.sprite = _borderNonSelected;

			var position = transform.localPosition;
			position.y = 0;
			transform.localPosition = position;
		}

		private void Select()
		{
			// TODO: SFX

			if (_isSelected) return;

			_callback?.Invoke(_id);
		}

		private void HandleRefreshAmmo(int clipSize, int ammo)
		{
			if (!_isSelected) return;

			RefreshAmmo(clipSize);
		}

		private void RefreshAmmo(int clipSize)
		{
			_txtAmmo.text = $"{clipSize}";
		}

		private void HandleStartReloading(float reloadingTime)
		{
			// TODO
		}

		private void HandleStopReloading()
		{
			// TODO
		}
	}
}