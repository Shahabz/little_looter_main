/*
 * Date: April 13th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
	public class UI_GameplayCheats_SwipePanel : MonoBehaviour
	{
        [SerializeField] private GameObject _content = default;
        [SerializeField] private TMPro.TMP_InputField _inputField_time = default;
        [SerializeField] private TMPro.TMP_InputField _inputField_distance = default;
        [SerializeField] private SwipeDetectionService _service = default;

        private bool _isOpen = false;
        private System.Action _callback = default;

        public bool IsOpen => _isOpen;

		private void Awake()
		{
            _inputField_time.onValueChanged.AddListener(TimeChanged);
            _inputField_distance.onValueChanged.AddListener(DistanceChanged);
        }

		private void OnDestroy()
		{
            _inputField_time.onValueChanged.RemoveAllListeners();
            _inputField_distance.onValueChanged.RemoveAllListeners();
        }

		public void Show(System.Action callback)
        {
            _callback = callback;

            _content.SetActive(true);

            _isOpen = true;
        }

        public void Hide()
        {
            _content.SetActive(false);

            _isOpen = false;

            _callback = null;
        }

        private void TimeChanged(string value)
		{
            if (!float.TryParse(value, out var amount)) return;

            _service.RefreshTime(amount);
		}

        private void DistanceChanged(string value)
        {
            if (!float.TryParse(value, out var amount)) return;

            _service.RefreshDistance(amount);
        }
    }
}