/*
 * Date: September 23th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_PlayerHealthPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private Slider _slider = default;
		[SerializeField] private Image _fill = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txt = default;
        [SerializeField] private PlayerHealth _playerHealth = default;
		[SerializeField] private Color _colorFull = default;
		[SerializeField] private Color _colorMedium = default;
		[SerializeField] private Color _colorLow = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			_playerHealth.OnInitialized += PlayerHealthInitialization;
			_playerHealth.OnTakeDamage += PlayerTakeDamage;
			_playerHealth.OnDead += PlayerDead;
		}

		private void OnDestroy()
		{
			_playerHealth.OnInitialized -= PlayerHealthInitialization;
			_playerHealth.OnTakeDamage -= PlayerTakeDamage;
			_playerHealth.OnDead -= PlayerDead;
		}

		#endregion

		#region Private methods

		private void PlayerHealthInitialization()
		{
			Refresh();
		}

		private void PlayerTakeDamage(float damage)
		{
			Refresh();
		}

		private void PlayerDead()
		{
			Refresh();

			_txt.enabled = false;
		}

		private void Refresh()
		{
			var hp = Mathf.CeilToInt(_playerHealth.Health);
			var maxHp = _playerHealth.MaxHealth;
			var progress = hp / maxHp;

			_slider.value = progress;

			_txt.text = $"{hp}/{maxHp}";

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

		#endregion
	}
}