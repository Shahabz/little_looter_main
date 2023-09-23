/*
 * Date: September 23th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using System;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_GameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _panel = default;
		[SerializeField] private PlayerHealth _playerHealth = default;
		[SerializeField] private float _delayToShow = 3;

		private void Awake()
		{
			_playerHealth.OnDead += PlayerDead;

			Hide();
		}

		private void OnDestroy()
		{
			_playerHealth.OnDead -= PlayerDead;
		}

		private void PlayerDead()
		{
			Invoke(nameof(Show), _delayToShow);
		}

		private void Show()
		{
			_panel.SetActive(true);
		}

		private void Hide()
		{
			_panel.SetActive(false);
		}
	}
}