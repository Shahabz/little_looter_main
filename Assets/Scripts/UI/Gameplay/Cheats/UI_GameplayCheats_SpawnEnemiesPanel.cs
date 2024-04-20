/*
 * Date: April 13th, 2024
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
	public class UI_GameplayCheats_SpawnEnemiesPanel : MonoBehaviour
	{
        [SerializeField] private GameObject _content = default;
        [SerializeField] private Button _btnApply = default;
        [SerializeField] private TMPro.TMP_InputField _inputField = default;

        private bool _isOpen = false;
        private int _amount = 0;
        private System.Action _spawnCallback = default;

        public bool IsOpen => _isOpen;

		private void Awake()
		{
            _btnApply.onClick.AddListener(Spawn);
            _inputField.onValueChanged.AddListener(AmountChanged);
		}

		private void OnDestroy()
		{
            _btnApply.onClick.RemoveAllListeners();
            _inputField.onValueChanged.RemoveAllListeners();
        }

		public void Show(System.Action callback)
        {
            _spawnCallback = callback;

            _content.SetActive(true);

            _isOpen = true;
        }

        public void Hide()
        {
            _content.SetActive(false);

            _isOpen = false;

            _spawnCallback = null;
        }

        private void AmountChanged(string value)
		{
            if (!int.TryParse(value, out var amount)) return;

            Debug.LogError($"Amount changed: <color=yellow>{amount}</color>");

            _amount = amount;
		}

        private void Spawn()
		{
            if (_amount <= 0) return;

            UI_GameplayEvents.OnSpawnEnemiesByCheat?.Invoke(_amount);

            _spawnCallback?.Invoke();
		}
    }
}