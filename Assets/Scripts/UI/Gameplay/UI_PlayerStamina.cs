/*
 * Date: April 25th, 2024
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_PlayerStamina : MonoBehaviour
    {
		[SerializeField] private Image[] _slots = default;

		private void Awake()
		{
			PlayerStaminaService.OnGrantPoints += HandleGrantPoints;
			PlayerStaminaService.OnConsumePoints += HandleConsumePoints;
		}

		private void OnDestroy()
		{
			PlayerStaminaService.OnGrantPoints -= HandleGrantPoints;
			PlayerStaminaService.OnConsumePoints -= HandleConsumePoints;
		}

		private void HandleGrantPoints(int points)
		{
			RefreshPoints(points);
		}

		private void HandleConsumePoints(int points)
		{
			RefreshPoints(points);
		}

		private void RefreshPoints(int points)
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				_slots[i].enabled = i < points;
			}
		}
	}
}