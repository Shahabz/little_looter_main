/*
 * Date: January 5th, 2024
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_GameplayCheats_SlotCameraPresets : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Button _btnApply = default;
        [SerializeField] private UI_GameplayCheats_SlotCamera _slotCameraAngle = default;
        [SerializeField] private UI_GameplayCheats_SlotAutoaiming _slotAutoaiming = default;
        [SerializeField] private UI_GameplayCheats_SlotCameraSize _slotCameraSize = default;
        [SerializeField] private UI_CameraRotatorPanel _rotatorPanel = default;
        [SerializeField] private UI_GameplayCheats_SlotAutofire _slotAutofire = default;

        #endregion

        #region Unity events

        private void OnEnable()
        {
            _btnApply.onClick.AddListener(Apply);
        }

        private void OnDisable()
        {
            _btnApply.onClick.RemoveAllListeners();
        }

        #endregion

        #region Private methods

        private void Apply()
        {
            _slotCameraAngle.ApplyPreset();
            _slotAutoaiming.ApplyPreset();
            _slotCameraSize.ApplyPreset();
            _rotatorPanel.ApplyPreset();
            _slotAutofire.ApplyPreset();
        }

		#endregion
	}
}