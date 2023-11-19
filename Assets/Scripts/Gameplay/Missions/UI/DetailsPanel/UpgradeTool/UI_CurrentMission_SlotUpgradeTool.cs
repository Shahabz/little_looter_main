/*
 * Date: November 18th, 2023
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_CurrentMission_SlotUpgradeTool : MonoBehaviour
    {
        [SerializeField] private Image _icon = default;
        [SerializeField] private GameObject _completed = default;

        public void Setup(Sprite icon, bool completed)
		{
            _icon.sprite = icon;

            _completed.SetActive(completed);
		}

        public void Completed()
		{
            _completed.SetActive(true);
        }
    }
}