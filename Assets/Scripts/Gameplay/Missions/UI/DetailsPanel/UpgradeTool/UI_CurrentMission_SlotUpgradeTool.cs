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
        [SerializeField] private TMPro.TextMeshProUGUI _txtAmount = default;
        [SerializeField] private GameObject _locked = default;

        public void Setup(Sprite icon, int amount, bool completed)
		{
            _icon.sprite = icon;

            _completed.SetActive(completed);
            _locked.SetActive(!completed);

            _txtAmount.text = $"{amount}";
		}

        public void Completed()
		{
            _completed.SetActive(true);

            _locked.SetActive(false);
        }

        public void RefreshAmount(int amount)
		{
            _txtAmount.text = $"{amount}";
        }
    }
}