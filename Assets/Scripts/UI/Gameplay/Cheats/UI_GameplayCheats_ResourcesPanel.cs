/*
 * Date: December 17th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_GameplayCheats_ResourcesPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _content = default;
        [SerializeField] private UI_GameplayCheats_ResourcesPanelSlot[] _slots = default;

        private bool _isOpen = false;

        public bool IsOpen => _isOpen;

        public void Show(PlayerProgressData progressData)
		{
            RefreshSlots(progressData);

            _content.SetActive(true);

            _isOpen = true;
        }

        public void Hide()
		{
            _content.SetActive(false);

            _isOpen = false;
		}

        private void RefreshSlots(PlayerProgressData progressData)
		{
			for (int i = 0; i < _slots.Length; i++)
			{
                var slot = _slots[i];

                slot.Setup(progressData);
			}
		}
    }
}