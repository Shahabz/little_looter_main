/*
 * Date: October 19th, 2023
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_GameplayCheatsPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private Button _btnAccess = default;
        [SerializeField] private GameObject _panel = default;
        [SerializeField] private Button _btnClosePanel = default;

		[SerializeField] private UI_GameplayCheats_SlotCamera _slotCameraAngle = default;
		[SerializeField] private UI_GameplayCheats_SlotAutoaiming _slotAutoaiming = default;
		[SerializeField] private UI_GameplayCheats_SlotCameraSize _slotCameraSize = default;
		[SerializeField] private UI_CameraRotatorPanel _rotatorPanel = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			Init();
		}

		private void OnDestroy()
		{
			Teardown();
		}

		#endregion

		#region Private methods

		private void Init()
		{
			_btnAccess.onClick.AddListener(OpenPanel);
			_btnClosePanel.onClick.AddListener(ClosePanel);

			ApplyPresets();
		}

		private void Teardown()
		{
			_btnAccess.onClick.RemoveAllListeners();
			_btnClosePanel.onClick.RemoveAllListeners();
		}

		private void OpenPanel()
		{
			_panel.SetActive(true);
		}

		private void ClosePanel()
		{
			_panel.SetActive(false);
		}

		private void ApplyPresets()
		{
			OpenPanel();

			_slotCameraAngle.ApplyPreset();
			_slotAutoaiming.ApplyPreset();
			_slotCameraSize.ApplyPreset();
			_rotatorPanel.ApplyPreset();

			Invoke(nameof(ClosePanel), 0.25f);
		}

		#endregion
	}
}