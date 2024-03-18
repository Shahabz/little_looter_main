/*
 * Date: March 18th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Ftue.View
{
    public class UI_Ftue_BasePanel : MonoBehaviour
    {
        [SerializeField] private GameObject _panel = default;

        public void Hide()
		{
            _panel.SetActive(false);
		}

        public void Show()
        {
            _panel.SetActive(true);
        }
    }
}