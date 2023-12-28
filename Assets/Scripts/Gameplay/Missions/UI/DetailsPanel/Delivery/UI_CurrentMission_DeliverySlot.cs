/*
 * Date: December 28th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{

    public class UI_CurrentMission_DeliverySlot : MonoBehaviour
    {
		[SerializeField] private TMPro.TextMeshProUGUI _txtAmount = default;
		[SerializeField] private Image _icon = default;
		[SerializeField] private GameObject _check = default;
		[SerializeField] private GameObject _locked = default;

		public void Setup(Sprite icon, int amount)
		{
			_txtAmount.text = $"{amount}";
			_icon.sprite = icon;
		}

        public void ShowLocked()
		{
			_check.SetActive(false);
			_locked.SetActive(true);
		}

        public void ShowCompleted()
		{
			_check.SetActive(true);
			_locked.SetActive(false);
		}
    }
}