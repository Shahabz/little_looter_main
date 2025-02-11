/*
 * Date: August 26th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using System;
using UnityEngine;

namespace LittleLooters.Gameplay
{
	public class Destructible : MonoBehaviour, ITakeDamage
	{
		[SerializeField] private GameObject _art = default;
		[SerializeField] private int _hp = default;
		[SerializeField] private int _maxHp = default;
		[SerializeField] private Collider _collider = default;

		public event Action OnInitialized;
		public event Action<int> OnTakeDamage;
		public event Action OnDead;

		public bool IsDead => _hp <= 0;
		public int Health => _hp;
		public int MaxHealth => _maxHp;

		public void Init(int initialHp, int maxHp)
		{
			_hp = initialHp;

			_maxHp = maxHp;
		}

		public void TakeDamage(int damage)
		{
			_hp = Mathf.Clamp(_hp - damage, 0, _hp);

			if (_hp > 0) return;

			Death();
		}

		private void Death()
		{
			_art.SetActive(false);

			_collider.enabled = false;
		}
	}
}