/*
 * Date: October 28th, 2023
 * Author: Peche
 */

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using LittleLooters.Gameplay.UI;

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

		[SerializeField] private int _id = default;
		[SerializeField] private DestructibleObjectData _data = default;
		[SerializeField] private GameObject _art = default;
		[SerializeField] private int _hp = default;
		[SerializeField] private int _maxHp = default;
		[SerializeField] private Collider _collider = default;

		[Header("UI")]
		[SerializeField] private GameObject _uiIndicator = default;
		[SerializeField] private GameObject _uiProgressBar = default;
		[SerializeField] private Slider _uiProgressBarFill = default;

		[Header("Damage Animation")]
		[SerializeField] private Vector3 _animDamagePunch = default;
		[SerializeField] private float _animDamageDuration = default;
		[SerializeField] [Range(0, 10)] private int _animDamageVibrato = default;
		[SerializeField] [Range(0, 1)] private float _animDamageElasticity = default;

		[Header("VFXs")]
		[SerializeField] private ParticleSystem _vfxHit = default;
		[SerializeField] private ParticleSystem _vfxDestruction = default;

		[Header("Hide on death")]
		[SerializeField] private GameObject[] _hideOnDeath = default;

		#endregion

		#region Private properties

		private System.Collections.Generic.List<DestructibleRewardData> _rewards = default;
		private bool _isAnimating = false;
		private const string _playerTag = "Player";

		#endregion

		#region Public properties

		public int Id => _data.Id;

		#endregion

		#region Public properties

		public int LevelRequired => _data.LevelRequired;

		#endregion

		#region ITakeDamage implementation

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

			UI_TextDamagePanel.OnAnimateDamage?.Invoke(transform.position, Mathf.FloorToInt(damage));

			var args = new DestructibleResourceApplyDamageArgs()
			{
				id = _data.Id,
				type = _data.Type
			};

			DestructibleResourceEvents.OnApplyDamage?.Invoke(args);

			if (_canDebug) DebugTakeDamage();

			RefreshHealthBar();

			ProcessDamageReward();

			if (_hp > 0)
			{
				AnimateDamage();

				ShowVfxDamage();

				return;
			}

			Death();
		}

		private void Death()
		{
			ShowVfxDestruction();

			_art.SetActive(false);

			_uiIndicator.SetActive(false);

			for (int i = 0; i < _hideOnDeath.Length; i++)
			{
				_hideOnDeath[i].SetActive(false);
			}

			_collider.enabled = false;

			DestructibleResourceEvents.OnDestroyed?.Invoke(_data.Id);

			OnDead?.Invoke();
		}

		#endregion

		#region Unity events

		private void Start()
		{
			// Init health based on data
			Init(_data.Hp, _data.Hp);

			InitRewards();

			// Hide indicator
			HideIndicator();

			// Hide progress bar
			HideProgressBar();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag(_playerTag)) return;

			ShowProximityIndicator();
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.CompareTag(_playerTag)) return;

			HideProximityIndicator();
		}

		#endregion

		#region Public methods

		public void Detected()
		{
			//ShowIndicator();

			ShowProgressBar();
		}

		public void Undetected()
		{
			//HideIndicator();

			HideProgressBar();
		}

		public void AnimateDamage()
		{
			if (_isAnimating) return;

			_isAnimating = true;

			_art.transform.DOPunchScale(_animDamagePunch, _animDamageDuration, _animDamageVibrato, _animDamageElasticity).OnComplete( () => _isAnimating = false );
		}

		#endregion

		#region Private methods

		private void InitRewards()
		{
			_rewards = _data.Rewards.ToList();
		}

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
			_uiProgressBarFill.value = ((float)_hp / (float)_maxHp);
		}

		private void ProcessDamageReward()
		{
			if (_rewards.Count <= 0) return;

			var damageProgress = 1 - ((float)_hp / (float)_maxHp);

			// Grant rewards
			for (int i = 0; i < _rewards.Count; i++)
			{
				var data = _rewards[i];

				if (data.percentage > damageProgress) continue;

				GrantReward(data);
			}

			// Remove granted rewards
			for (int i = 0; i < _rewards.Count; i++)
			{
				var data = _rewards[i];

				if (data.percentage > damageProgress) continue;

				_rewards.RemoveAt(i);
			}
		}

		private void GrantReward(DestructibleRewardData data)
		{
			var damageProgress = Mathf.FloorToInt((1 - (_hp / _maxHp)) * 100);

			if (_canDebug) DebugGrantReward(data.resource.DisplayName, data.amount, damageProgress);

			DestructibleResourceEvents.OnGrantRewardsByDamage?.Invoke(data.resource.Id, data.amount);
		}

		private void ShowVfxDamage()
		{
			_vfxHit.Play();
		}

		private void ShowVfxDestruction()
		{
			_vfxDestruction.Play();
		}

		private void ShowProximityIndicator()
		{
			if (IsDead) return;

			_uiIndicator.SetActive(true);
		}

		private void HideProximityIndicator()
		{
			_uiIndicator.SetActive(false);
		}

		#endregion

		#region Debug

		private bool _canDebug = false;

		private void DebugGrantReward(string displayName, int amount, float progress)
		{
			Debug.LogError($"Grant reward <color=yellow>{displayName}</color>, <color=cyan>{amount}</color>, damage: %<color=magenta>{progress}</color>");
		}

		private void DebugTakeDamage()
		{
			Debug.LogError($"Destructible [<color=yellow>{_id}</color>][<color=magenta>{_data.Type}</color>] take damage");
		}

		#endregion
	}
}