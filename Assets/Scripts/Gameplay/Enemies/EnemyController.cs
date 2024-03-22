/*
 * Date: September 16th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace LittleLooters.Gameplay
{
	/// <summary>
	/// In charge of controlling the state of the entity and communicate with the visual controller to visualize the current state.
	/// Compose by:
	///		- weapon controller
	///		- visual controller
	///		- config data
	///		- behavior service
	/// </summary>
	public class EnemyController : MonoBehaviour, ITakeDamage
	{
		public Action OnWasDetected;
		public Action OnDetectionFinished;

		#region Inspector

		[SerializeField] private EnemyConfiguration _config = default;
		[SerializeField] private EnemyBehaviorData _data = default;
		[SerializeField] private int _level = 1;
		[SerializeField] private EnemyState _state = EnemyState.NONE;
		[SerializeField] private VisualEnemyController _visualController = default;
		[SerializeField] private WeaponEnemyController _weaponController = default;
		[SerializeField] private Transform _target = default;
		[SerializeField] private FieldOfViewHelper _fovHelper = default;
		[SerializeField] private Collider _collider = default;
		[SerializeField] private GameObject _mainCamera = default;
		[SerializeField] private GameObject _detection = default;
		[SerializeField] private float _rotationSmoothTime = 0.12f;
		[SerializeField] private UI_EnemyHud _hud = default;
		[SerializeField] private bool _canShowTextDamage = false;

		#endregion

		#region Private properties

		private int _id = -1;
		private bool _enabled = false;
		private int _hp = default;
		private int _maxHp = default;
		private EnemyFieldOfViewService _fovService = default;
		private NavMeshAgent _agent = default;
		private float _nextAttackTime = 0;
		private EnemyState _previousState = EnemyState.NONE;
		private float _refreshTime = 0;
		private const float DELAY_CALCULATION = 0.12f;
		private ITakeDamage _targetHealth = default;
		private bool _isPerformingAnAttack = false;
		private bool _hudExist = false;

		#endregion

		#region Public properties

		public int Id => _id;

		#endregion

		#region Unity events

		private void OnBecameVisible()
		{
			ShowHUD();
		}

		private void OnBecameInvisible()
		{
			HideHUD();
		}

		#endregion

		#region Public methods

		public void Initialization(int id)
		{
			_id = id;

			var levelConfiguration = _config.GetLevelConfiguration(_level);

			_hp = levelConfiguration.hp;
			_maxHp = _hp;

			_agent = GetComponent<NavMeshAgent>();

			_agent.speed = _data.WalkingSpeed;

			_fovService = new EnemyFieldOfViewService();

			_fovService.Init(transform, _target, _data);

			_fovService.OnStartDetection += OnStartDetection;
			_fovService.OnStopDetection += OnStopDetection;

			_fovHelper.Init(_data);

			_state = EnemyState.IDLE;

			_visualController.Init(_state, _data);

			if (_target.TryGetComponent<ITakeDamage>(out _targetHealth))
			{
				_targetHealth.OnDead += TargetDead;
			}

			InitHUD();

			_weaponController.Init(_data, levelConfiguration, _target, MeleeAttackCompleted, MeleeAttackStarted);

			MarkAsNonDetected();

			if (_canDebug) DebugInitialization(levelConfiguration);

			_enabled = true;
		}

		public void Teardown()
		{
			if (_fovService != null)
			{
				_fovService.OnStartDetection -= OnStartDetection;
				_fovService.OnStopDetection -= OnStopDetection;
			}

			if (_targetHealth != null)
			{
				_targetHealth.OnDead -= TargetDead;
			}
		}
		
		public void Tick(float deltaTime)
		{
			if (!_enabled) return;

			if (_canDebug) _fovHelper.Tick(transform.forward);

			RefreshDetection();

			RefreshRotation();

			RefreshAttackState();

			_visualController.Refresh(_state, _isPerformingAnAttack);

			DebugRefreshState();

			_refreshTime -= deltaTime;

			if (_refreshTime > 0) return;

			_refreshTime = DELAY_CALCULATION;

			_fovService.Tick();
		}

		public void MarkAsDetected()
		{
			_detection.SetActive(true);

			OnWasDetected?.Invoke();
		}

		public void MarkAsNonDetected()
		{
			_detection.SetActive(false);

			OnDetectionFinished?.Invoke();
		}

		#endregion

		#region Private methods

		private void OnStartDetection()
		{
			if (!_enabled) return;

			if (_state == EnemyState.ATTACK) return;

			_agent.SetDestination(_target.position);
			_agent.isStopped = false;

			_state = EnemyState.CHASE;
		}

		private void RefreshDetection()
		{
			if (!_enabled) return;

			if (_state != EnemyState.CHASE) return;

			_agent.SetDestination(_target.position);
			_agent.isStopped = false;
		}

		private void OnStopDetection()
		{
			if (!_enabled) return;

			StopMovement();

			_state = EnemyState.IDLE;
		}

		private void RefreshRotation()
		{
			if (!_fovService.TargetDetected) return;

			RotateTowardsTarget();
		}

		private void RotateTowardsTarget()
		{
			var targetPosition = _target.position;
			targetPosition.y = transform.position.y;

			var targetDirection = _target.position - transform.position;

			transform.rotation = Quaternion.LookRotation(targetDirection);
		}

		private void RefreshAttackState()
		{
			var inAttackRange = CheckInAttackRange();

			// It was attacking but is not in attack range area
			if (_state == EnemyState.ATTACK && !inAttackRange)
			{
				// Should wait until the attack in progress has finished
				if (_isPerformingAnAttack) return;

				StopAttackState();
				return;
			}

			// It isn't attacking and it is in attack range area
			if (_state != EnemyState.ATTACK && inAttackRange)
			{
				StartAttackState();
				return;
			}

			// It is not in attack state
			if (_state != EnemyState.ATTACK) return;

			// It is in attack state and inside attack area
			Attack();
		}

		private bool CheckInAttackRange()
		{
			var currentPosition = transform.position;

			var targetPosition = _target.position;
			targetPosition.y = currentPosition.y;

			Vector3 directionToTarget = targetPosition - currentPosition;

			var distance = directionToTarget.magnitude;

			var insideAttackRange = distance <= _data.RadiusAttack;

			return insideAttackRange;
		}

		private void StopAttackState()
		{
			_state = (_fovService.TargetDetected) ? EnemyState.CHASE : EnemyState.IDLE;
		}

		private void StartAttackState()
		{
			_state = EnemyState.ATTACK;

			StopMovement();

			Attack();
		}

		private void Attack()
		{
			if (Time.time < _nextAttackTime) return;

			_nextAttackTime = Time.time + _data.AttackRate;

			//Debug.LogError("Attack!");

			_visualController.Attack();
		}

		private void StopMovement()
		{
			_agent.isStopped = true;
			_agent.velocity = Vector3.zero;
		}

		private void DebugRefreshState()
		{
			if (_state == _previousState) return;

			//Debug.LogError($"Move from <color=yellow>{_previousState}</color> to <color=cyan>{_state}</color>");

			_previousState = _state;
		}

		private void RefreshDeathState()
		{
			_state = EnemyState.DIE;

			_visualController.Refresh(_state, false);

			DebugRefreshState();
		}

		private void TargetDead()
		{
			if (!_enabled) return;

			_enabled = false;

			StopMovement();

			_state = EnemyState.IDLE;

			_visualController.Refresh(_state, false);
		}

		private void MeleeAttackStarted()
		{
			_isPerformingAnAttack = true;
		}

		private void MeleeAttackCompleted()
		{
			_isPerformingAnAttack = false;
		}

		private void InitHUD()
		{
			_hudExist = _hud != null;

			if (!_hudExist) return;

			_hud.Init(_level, _hp, _maxHp);
		}

		private void RefreshHudByDamage()
		{
			if (!_hudExist) return;

			_hud.RefreshHealth((float)_hp / (float)_maxHp);
		}

		private void RefreshHudByDeath()
		{
			if (!_hudExist) return;

			_hud.HandleDead();
		}

		private void ShowHUD()
		{
			if (!_hudExist) return;

			_hud.Show();
		}

		private void HideHUD()
		{
			if (!_hudExist) return;

			_hud.Hide();
		}

		#endregion

		#region Health system

		public event Action OnInitialized;
		public event Action<int> OnTakeDamage;
		public event Action OnDead;

		public bool IsDead => _hp <= 0;
		public int Health => _hp;
		public int MaxHealth => _maxHp;
		public int Level => _level;

		public void Init(int initialHp, int maxHp)
		{
			//_hp = initialHp;
			//_maxHp = maxHp;
		}

		public void TakeDamage(int damage)
		{
			if (_canShowTextDamage)
			{
				UI.UI_TextDamagePanel.OnAnimateDamage?.Invoke(transform.position, Mathf.FloorToInt(damage));
			}

			_hp = Mathf.Clamp(_hp - damage, 0, _hp);

			RefreshHudByDamage();
			
			if (_hp > 0)
			{
				_visualController.TakeDamage();

				return;
			}

			Death();
		}

		private void Death()
		{
			_collider.enabled = false;

			_enabled = false;

			StopMovement();

			RefreshDeathState();

			RefreshHudByDeath();

			OnDead?.Invoke();
		}

		#endregion

		#region Debug

		private bool _canDebug = false;

		private void DebugInitialization(EnemyLevelConfiguration levelConfiguration)
		{
			Debug.LogError($"Enemy::Initialization -> id: <color=yellow>{_config.id}</color>, level: <color=cyan>{_level}</color>, damage: [<color=magenta>{levelConfiguration.minDamage}-{levelConfiguration.maxDamage}</color>], name: {gameObject.name}");
		}

		#endregion
	}
}