/*
 * Date: December 27th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
	/// <summary>
	/// Logics that represents the floating player's health bar
	/// </summary>
    public class UI_Player_HudHealthBar : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _content = default;
		[SerializeField] private Slider _slider = default;
		[SerializeField] private Image _fill = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txt = default;
		[SerializeField] private Color _colorFull = default;
		[SerializeField] private Color _colorMedium = default;
		[SerializeField] private Color _colorLow = default;

		#endregion

		#region Private properties

		private PlayerHealth _playerHealth = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			_playerHealth = GetComponentInParent<PlayerHealth>();

			_playerHealth.OnInitialized += HandleOnPlayerHealthInitialization;
			_playerHealth.OnTakeDamage += HandleOnPlayerTakeDamage;
			_playerHealth.OnDead += HandleOnPlayerDead;

			LevelEnemies.OnStartDetection += HandleStartEnemiesDetection;
			LevelEnemies.OnStopDetection += HandleStopEnemiesDetection;
		}

		private void Start()
		{
			HidePanel();
		}

		private void OnDestroy()
		{
			_playerHealth.OnInitialized -= HandleOnPlayerHealthInitialization;
			_playerHealth.OnTakeDamage -= HandleOnPlayerTakeDamage;
			_playerHealth.OnDead -= HandleOnPlayerDead;

			LevelEnemies.OnStartDetection -= HandleStartEnemiesDetection;
			LevelEnemies.OnStopDetection -= HandleStopEnemiesDetection;
		}

		#endregion

		#region Private methods

		private void ShowPanel()
		{
			_content.SetActive(true);
		}

		private void HidePanel()
		{
			_content.SetActive(false);
		}

		private void HandleOnPlayerHealthInitialization()
		{
			Refresh();
		}

		private void HandleOnPlayerTakeDamage(float damage)
		{
			Refresh();
		}

		private void HandleOnPlayerDead()
		{
			_content.SetActive(false);
		}

		private void Refresh()
		{
			var hp = Mathf.CeilToInt(_playerHealth.Health);
			var maxHp = _playerHealth.MaxHealth;
			var progress = (float)hp / (float)maxHp;

			_slider.value = progress;

			_txt.text = $"{hp}";

			RefreshColor(progress);
		}

		private void RefreshColor(float progress)
		{
			// Health is ok
			if (progress >= 0.75f)
			{
				_fill.color = _colorFull;
				return;
			}

			// Health is not ok
			if (progress >= 0.25f)
			{
				_fill.color = _colorMedium;
				return;
			}

			// Health is in dangereous values
			_fill.color = _colorLow;
		}

		private void HandleStopEnemiesDetection()
		{
			HidePanel();
		}

		private void HandleStartEnemiesDetection()
		{
			ShowPanel();
		}

		#endregion
	}
}