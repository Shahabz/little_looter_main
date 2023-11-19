/*
 * Date: November 18th, 2023
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_CurrentMission_SlotDestruction : MonoBehaviour
    {
        [SerializeField] private Image _icon = default;

        public void Setup(Sprite icon)
		{
            _icon.sprite = icon;
		}
    }
}