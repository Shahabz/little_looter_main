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
		[SerializeField] private GameObject _progressBarAmmo = default;
		[SerializeField] private Image _progressBarAmmoFill = default;
		[SerializeField] private Button _btn = default;
		[SerializeField] private GameObject _selection = default;

		[Header("Reload info")]
		[SerializeField] private GameObject _reloadPanel = default;
		[SerializeField] private Image _reloadProgressBar = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtReloadingTime = default;

		[Header("Interaction bar")]
		[SerializeField] private GameObject _interactionBarPanel = default;
		[SerializeField] private Image _interactionBarFill = default;

		#endregion

		#region Private properties

		private string _id = string.Empty;
		private bool _isSelected = false;
		private Action<string> _callback = default;
		private bool _isReloading = false;
		private int _magazine = 0;

		#endregion

		#region Public properties

		public string Id => _id;

		#endregion

		#region Unity events

		private void OnEnable()
		{
			SubscribeEvents();
		}

		private void OnDisable()
		{
			UnsubscribeEvents();
		}

		private void Update()
		{
			if (!_isRefreshingInteractionProgress && !_isRefreshingInteractionDelay) return;

			RefreshInteractionProgress();
		}

		#endregion

		#region Public methods

		public void Init(PlayerWeaponInfo weaponInfo, bool isSelected, Action<string> callback, int magazine)
		{
			_id = weaponInfo.id;

			_callback = callback;

			_magazine = magazine;

			_btn.onClick.AddListener(Select);

			//SubscribeEvents();

			_txtAmmo.text = $"{weaponInfo.ammo}";

			RefreshProgressBar(weaponInfo.ammo);

			HideInteractionProgressBar();

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

			//UnsubscribeEvents();

			_btn.onClick.RemoveAllListeners();
		}

        public void MarkAsSelected()
		{
			_isSelected = true;

			_selection.SetActive(_isSelected);

			var position = transform.localPosition;
			position.y = 20;

			transform.localPosition = position;
		}

		public void MarkAsNonSelected()
		{
			_isSelected = false;

			_selection.SetActive(_isSelected);

			var position = transform.localPosition;
			position.y = 0;
			transform.localPosition = position;
		}

		#endregion

		#region Private methods

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

			ShowAmmoProgressBar();
		}

		private void HandleWeaponStartReloading(PlayerProgressEvents.WeaponStartReloadingArgs args)
		{
			if (!args.id.Equals(_id)) return;

			_reloadPanel.SetActive(true);

			_isReloading = true;

			HideAmmoProgressBar();

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

			RefreshProgressBar(args.ammo);
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

		private void RefreshProgressBar(int ammo)
		{
			_progressBarAmmoFill.fillAmount = (float)ammo / _magazine;
		}

		private void ShowAmmoProgressBar()
		{
			_progressBarAmmo.SetActive(true);
		}

		private void HideAmmoProgressBar()
		{
			_progressBarAmmo.SetActive(false);
		}

		#endregion

		#region IPointer implementation

		private float _interactionProgressTime = 0;
		private float _interactionProgressRemainingTime = 0;
		private float _interactionProgressTotalTime = 1.5f;	// Interaction progress bar time duration
		private float _interactionProgressDelay = 0.5f;		// Time before showing interaction progress bar
		private bool _isRefreshingInteractionProgress = false;
		private bool _isRefreshingInteractionDelay = false;

		public void OnPointerDown(PointerEventData eventData)
		{
			if (_isReloading) return;

			_isRefreshingInteractionDelay = true;
			_interactionProgressRemainingTime = _interactionProgressDelay;
			_isRefreshingInteractionProgress = false;
			_interactionProgressTime = 0;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_isRefreshingInteractionDelay = false;
			_interactionProgressRemainingTime = 0;
			_isRefreshingInteractionProgress = false;
			_interactionProgressTime = 0;

			if (_isReloading) return;

			HideInteractionProgressBar();
		}

		private void RefreshInteractionProgress()
		{
			if (_isRefreshingInteractionDelay)
			{
				_interactionProgressRemainingTime -= Time.deltaTime;

				if (_interactionProgressRemainingTime > 0) return;

				_isRefreshingInteractionDelay = false;

				_isRefreshingInteractionProgress = true;

				_interactionProgressTime = _interactionProgressTotalTime;

				ShowInteractionProgressBar();

				return;
			}

			if (_isRefreshingInteractionProgress)
			{
				_interactionProgressTime -= Time.deltaTime;

				RefreshProgressBar();

				if (_interactionProgressTime > 0) return;

				_isRefreshingInteractionProgress = false;

				HideInteractionProgressBar();

				StartReloading();

				return;
			}
		}

		private void ShowInteractionProgressBar()
		{
			RefreshProgressBar();

			_interactionBarPanel.SetActive(true);
		}

		private void HideInteractionProgressBar()
		{
			_interactionBarPanel.SetActive(false);
		}

		private void RefreshProgressBar()
		{
			var progress = 1 - _interactionProgressTime / _interactionProgressTotalTime;

			_interactionBarFill.fillAmount = progress;
		}

		private void StartReloading()
		{
			_isReloading = true;

			UI_GameplayEvents.OnWeaponStartReloading?.Invoke(_id);
		}

		#endregion
	}
}