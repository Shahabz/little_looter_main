/*
 * Date: November 5th, 2023
 * Author: Peche
 */

using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_GameplayCheats_SlotCameraSize : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Button _btnApply = default;
        [SerializeField] private TextMeshProUGUI _txtSize = default;
        [SerializeField] private CinemachineVirtualCamera _camera = default;
        [SerializeField] private float[] _optionValues = default;

        #endregion

        #region Private properties

        private int _currentOption = 0;

        #endregion

        #region Unity events

        private void OnEnable()
        {
            _btnApply.onClick.AddListener(Apply);

            RefreshSize();
        }

        private void OnDisable()
        {
            _btnApply.onClick.RemoveAllListeners();
        }

		#endregion

		#region Public methods

        public void ApplyPreset()
		{
            _currentOption = 1;

            Apply();
		}

		#endregion

		#region Private methods

		private void Apply()
        {
            _currentOption = (_currentOption + 1) % _optionValues.Length;

            var cameraSize = _optionValues[_currentOption];

            _camera.m_Lens.OrthographicSize = cameraSize;

            RefreshSize();
        }

        private void RefreshSize()
        {
            var cameraSize = _optionValues[_currentOption];

            _txtSize.text = $"[{cameraSize}]";
        }

#endregion
    }
}