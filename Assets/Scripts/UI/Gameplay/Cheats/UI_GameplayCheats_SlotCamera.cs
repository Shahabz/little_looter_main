/*
 * Date: October 19th, 2023
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_GameplayCheats_SlotCamera : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Button _btnApply = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtAngle = default;
		[SerializeField] private Transform _camera = default;
        [SerializeField] private int[] _optionValues = default;

        #endregion

        #region Private properties

        private int _currentOption = 0;

		#endregion

		#region Unity events

		private void OnEnable()
		{
            _btnApply.onClick.AddListener(Apply);

			RefreshAngle();
		}

		private void OnDisable()
		{
			_btnApply.onClick.RemoveAllListeners();
		}

		#endregion

		#region Public methods

		public void ApplyPreset()
		{
			_currentOption = 2;

			Apply();
		}

		#endregion

		#region Private methods

		private void Apply()
		{
			_currentOption = (_currentOption+1) % _optionValues.Length;

			var angle = _optionValues[_currentOption];

			_camera.localRotation = Quaternion.Euler(angle, 50, 0);

			RefreshAngle();
		}

		private void RefreshAngle()
		{
			var angle = _optionValues[_currentOption];

			_txtAngle.text = $"[{angle}]";
		}

		#endregion
	}
}