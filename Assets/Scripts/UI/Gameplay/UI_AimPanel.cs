/*
 * Date: August 26th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
	public class UI_AimPanel : MonoBehaviour
	{
		#region Inspector

		[SerializeField] private StarterAssets.StarterAssetsInputs _input = default;
		[SerializeField] private RectTransform _panel = default;
		[SerializeField] private Transform _target = default;
		[SerializeField] private Camera _camera = default;
		[SerializeField] private Vector2 _offset = default;

		#endregion

		#region Private properties

		private bool _isAiming = false;

		#endregion

		#region Unity events

		private void Start()
		{
			Hide();
		}

		private void Update()
		{
			RefreshVisibility();

			if (!_isAiming) return;

			FollowTarget();
		}

		#endregion

		#region Private methods

		private void RefreshVisibility()
		{
			if (_input.StartAiming)
			{
				Show();
				
				_isAiming = true;

				return;
			}

			if (_input.StopAiming)
			{
				Hide();

				_isAiming = false;
			}
		}

		private void FollowTarget()
		{
			_panel.position = RectTransformUtility.WorldToScreenPoint(_camera, _target.position) + _offset;
		}

		private void Hide()
		{
			_panel.gameObject.SetActive(false);
		}

		private void Show()
		{
			_panel.gameObject.SetActive(true);
		}

		#endregion
	}
}