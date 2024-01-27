/*
 * Date: September 23th, 2023
 * Author: Peche
 */

using System;
using UnityEngine;

namespace LittleLooters.Gameplay.Combat
{
    public class PlayerHealth : MonoBehaviour, ITakeDamage
    {
		#region Events

		public event Action OnInitialized;
		public event Action<int> OnTakeDamage;
		public event Action OnDead;

		#endregion

		#region Inspector

		[SerializeField] private int _hp = default;
		[SerializeField] private int _maxHp = default;

		#endregion

		private bool _godMode = false;

		#region ITakeDamage implementation

		public bool IsDead => _hp <= 0;
		public int Health => _hp;
		public int MaxHealth => _maxHp;

		public void Init(int initialHp, int maxHp)
		{
			_hp = initialHp;

			_maxHp = maxHp;

			OnInitialized?.Invoke();
		}

		public void TakeDamage(int damage)
		{
			if (IsDead) return;

			if (!_godMode)
			{
				_hp = Mathf.Clamp(_hp - damage, 0, _hp);
			}

			UI.UI_TextDamagePanel.OnAnimateDamage?.Invoke(transform.position, Mathf.FloorToInt(damage));

			if (_hp > 0)
			{
				OnTakeDamage?.Invoke(damage);

				return;
			}

			OnDead?.Invoke();

			UI_GameplayEvents.OnPlayerHasDie?.Invoke();
		}

		#endregion

		public void SetGodMode(bool modeOn)
		{
			_godMode = modeOn;
		}

		[ContextMenu("Apply Damage")]
		public void ApplyDamageForTesting()
		{
			var damage = Mathf.FloorToInt(UnityEngine.Random.Range(5f, 10f));

			TakeDamage(damage);
		}

		[ContextMenu("Kill")]
		public void KillForTesting()
		{
			var damage = _maxHp;

			TakeDamage(damage);
		}
	}
}