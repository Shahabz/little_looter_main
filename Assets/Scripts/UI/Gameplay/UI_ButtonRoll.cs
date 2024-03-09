/*
 * Date: March 9th, 2023
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_ButtonRoll : MonoBehaviour
    {
		[SerializeField] private Image _fill = default;

		private bool _isProcessing = false;
		private float _remainingTime = 0;
		private float _duration = 0;
		private bool _gameStarted = false;

		private void Awake()
		{
			UI_GameplayEvents.OnStartGame += HandleGameStarted;
			UI_GameplayEvents.OnStartRolling += HandlePlayerRollStarted;
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnStartGame -= HandleGameStarted;
			UI_GameplayEvents.OnStartRolling -= HandlePlayerRollStarted;
		}

		private void Update()
		{
			if (!_gameStarted) return;

			if (!_isProcessing) return;

			RefreshProgress();
		}

		private void HandleGameStarted()
		{
			_gameStarted = true;
		}

		private void HandlePlayerRollStarted(float duration)
		{
			if (!_gameStarted) return;

			_isProcessing = true;

			_remainingTime = duration;
			_duration = duration;

			RefreshProgress();
		}

		private void RefreshProgress()
		{
			_remainingTime -= Time.deltaTime;

			var progress = 1 - (_remainingTime / _duration);

			_fill.fillAmount = progress;

			if (_remainingTime > 0) return;

			_isProcessing = false;

			_remainingTime = 0;
		}
	}
}