/*
 * Date: October 28th, 2023
 * Author: Peche
 */

using System;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay
{
    /// <summary>
    /// Represents an object that can be destroyed by player's melee action when it is near enough.
    /// Each object has data related with:
    ///   - melee weapon level required,
    ///   - hp,
    ///   - total reward resources,
    ///   - amount of rewards based on damage applied
    /// </summary>
    public class DestructibleResourceObject : MonoBehaviour, Combat.ITakeDamage
    {
		#region Inspector

		[SerializeField] private GameObject _art = default;
		[SerializeField] private float _hp = default;
		[SerializeField] private float _maxHp = default;
		[SerializeField] private Collider _collider = default;

		[Header("UI")]
		[SerializeField] private GameObject _uiIndicator = default;
		[SerializeField] private GameObject _uiProgressBar = default;
		[SerializeField] private Slider _uiProgressBarFill = default;

		#endregion

		#region ITakeDamage implementation

		public event Action OnInitialized;
		public event Action<float> OnTakeDamage;
		public event Action OnDead;

		public bool IsDead => _hp <= 0;
		public float Health => _hp;
		public float MaxHealth => _maxHp;

		public void Init(float initialHp, float maxHp)
		{
			_hp = initialHp;

			_maxHp = maxHp;
		}

		public void TakeDamage(float damage)
		{
			_hp = Mathf.Clamp(_hp - damage, 0, _hp);

			RefreshHealthBar();

			if (_hp > 0) return;

			Death();
		}

		private void Death()
		{
			_art.SetActive(false);

			_collider.enabled = false;
		}

		#endregion

		#region Unity events

		private void Start()
		{
			HideIndicator();

			HideProgressBar();
		}

		#endregion

		#region Public methods

		public void Detected()
		{
			ShowIndicator();

			ShowProgressBar();
		}

		public void Undetected()
		{
			HideIndicator();

			HideProgressBar();
		}

		#endregion

		#region Private methods

		private void ShowIndicator()
		{
			_uiIndicator.SetActive(true);
		}

		private void HideIndicator()
		{
			_uiIndicator.SetActive(false);
		}
		
		private void HideProgressBar()
		{
			_uiProgressBar.SetActive(false);
		}

		private void ShowProgressBar()
		{
			_uiProgressBar.SetActive(true);

			RefreshHealthBar();
		}

		private void RefreshHealthBar()
		{
			_uiProgressBarFill.value = (_hp / _maxHp);
		}

		#endregion
	}
}