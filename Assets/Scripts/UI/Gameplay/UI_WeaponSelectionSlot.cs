/*
 * Date: September 30th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using LittleLooters.Model;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_WeaponSelectionSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		#region Inspector

		[SerializeField] private TMPro.TextMeshProUGUI _txtAmmo = default;
		[SerializeField] private Button _btn = default;
		[SerializeField] private Image _border = default;
		[SerializeField] private Sprite _borderSelected = default;
		[SerializeField] private Sprite _borderNonSelected = default;

		[Header("Reload info")]
		[SerializeField] private GameObject _reloadPanel = default;
		[SerializeField] private Image _reloadProgressBar = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtReloadingTime = default;

		#endregion

		#region Private properties

		private string _id = string.Empty;
		private bool _isSelected = false;
		private Action<string> _callback = default;
		private bool _isReloading = false;

		#endregion

		#region Public properties

		public string Id => _id;

		#endregion

		public void Init(PlayerWeaponInfo weaponInfo, bool isSelected, Action<string> callback)
		{
			_id = weaponInfo.id;

			_callback = callback;

			_btn.onClick.AddListener(Select);

			SubscribeEvents();

			_txtAmmo.text = $"{weaponInfo.ammo}";

			if (isSelected)
			{
				MarkAsSelected();
				return;
			}

			MarkAsNonSelected();
		}

        public void Teardown()
		{
			_callback = null;

			UnsubscribeEvents();

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

			if (_isReloading) return;

			_callback?.Invoke(_id);
		}

		private void SubscribeEvents()
		{
			PlayerProgressEvents.OnWeaponAmmoChanged += HandleWeaponAmmoChanged;
			PlayerProgressEvents.OnWeaponSelectionChanged += HandleWeaponSelectionChanged;
			PlayerProgressEvents.OnWeaponStartReloading += HandleWeaponStartReloading;
			PlayerProgressEvents.OnWeaponStopReloading += HandleWeaponStopReloading;
		}

		private void UnsubscribeEvents()
		{
			PlayerProgressEvents.OnWeaponAmmoChanged -= HandleWeaponAmmoChanged;
			PlayerProgressEvents.OnWeaponSelectionChanged -= HandleWeaponSelectionChanged;
			PlayerProgressEvents.OnWeaponStartReloading -= HandleWeaponStartReloading;
			PlayerProgressEvents.OnWeaponStopReloading -= HandleWeaponStopReloading;
		}

		private void HandleWeaponStopReloading(PlayerProgressEvents.WeaponStopReloadingArgs args)
		{
			if (!args.id.Equals(_id)) return;

			_reloadPanel.SetActive(false);

			_isReloading = false;
		}

		private void HandleWeaponStartReloading(PlayerProgressEvents.WeaponStartReloadingArgs args)
		{
			if (!args.id.Equals(_id)) return;

			_reloadPanel.SetActive(true);

			_isReloading = true;

			StartCoroutine(Reload(args.duration));
		}

		private void HandleWeaponSelectionChanged(PlayerProgressEvents.WeaponSelectionArgs args)
		{
			if (!args.id.Equals(_id)) return;

			// TODO: selection based on args.isSelected
		}

		private void HandleWeaponAmmoChanged(PlayerProgressEvents.WeaponAmmoChangeArgs args)
		{
			if (!args.id.Equals(_id)) return;

			_txtAmmo.text = $"{args.ammo}";
		}

		private IEnumerator Reload(float duration)
		{
			var remainingTime = duration;

			_reloadProgressBar.fillAmount = 0;

			_txtReloadingTime.text = UI_Utils.GetFormatTime(Mathf.CeilToInt(remainingTime));

			while (remainingTime > 0)
			{
				remainingTime -= Time.deltaTime;

				_reloadProgressBar.fillAmount = 1 - (remainingTime / duration);

				_txtReloadingTime.text = UI_Utils.GetFormatTime(Mathf.CeilToInt(remainingTime));

				yield return null;
			}

			_reloadProgressBar.fillAmount = 1;

			_txtReloadingTime.text = UI_Utils.GetFormatTime(0);
		}

		#region IPointer implementation

		private float _timePressed = 0;
		private bool _isPressed = false;
		private float _timeToStartReloading = 2;
		private float _stepTime = 0.5f;

		public void OnPointerDown(PointerEventData eventData)
		{
			_isPressed = true;
			_timePressed = 0;

			InvokeRepeating(nameof(CheckStartReloadingByPressing), 0, _stepTime);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_isPressed = false;
			_timePressed = 0;

			CancelInvoke(nameof(CheckStartReloadingByPressing));
		}

		private void CheckStartReloadingByPressing()
		{
			if (!_isPressed) return;

			_timePressed += _stepTime;

			if (_timePressed < _timeToStartReloading) return;

			_isReloading = true;

			UI_GameplayEvents.OnWeaponStartReloading?.Invoke(_id);

			CancelInvoke(nameof(CheckStartReloadingByPressing));
		}

		#endregion
	}
}