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
		#region Inspector

		[SerializeField] private EnemyBehaviorData _data = default;
		[SerializeField] private VisualEnemyController _visualController = default;
		[SerializeField] private WeaponEnemyController _weaponController = default;
		[SerializeField] private Transform _target = default;
		[SerializeField] private FieldOfViewHelper _fovHelper = default;
		[SerializeField] private float _hp = default;
		[SerializeField] private float _maxHp = default;
		[SerializeField] private Collider _collider = default;
		[SerializeField] private bool _canDebug = false;

		#endregion

		#region Private properties

		private bool _enabled = false;
		private EnemyFieldOfViewService _fovService = default;
		private NavMeshAgent _agent = default;
		private EnemyState _state = EnemyState.NONE;
		private float _nextAttackTime = 0;
		private EnemyState _previousState = EnemyState.NONE;
		private float _refreshTime = 0;
		private const float DELAY_CALCULATION = 0.12f;
		private ITakeDamage _targetHealth = default;

		#endregion

		#region Public methods

		public void Initialization()
		{
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

		#endregion

		#region Unity events

		private void Update()
		{
			if (!_enabled) return;

			if (_canDebug) _fovHelper.Tick(transform.forward);

			RefreshDetection();

			RefreshAttackState();

			_visualController.Refresh(_state);

			DebugRefreshState();

			_refreshTime -= Time.deltaTime;

			if (_refreshTime > 0) return;

			_refreshTime = DELAY_CALCULATION;

			_fovService.Tick();

			RefreshRotation();
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

			Debug.LogError("Attack!");
		}

		private void StopMovement()
		{
			_agent.isStopped = true;
		}

		private void DebugRefreshState()
		{
			if (_state == _previousState) return;

			Debug.LogError($"Move from <color=yellow>{_previousState}</color> to <color=cyan>{_state}</color>");

			_previousState = _state;
		}

		private void RefreshDeathState()
		{
			_state = EnemyState.DIE;

			_visualController.Refresh(_state);

			DebugRefreshState();
		}

		private void TargetDead()
		{
			if (!_enabled) return;

			_enabled = false;

			StopMovement();

			_state = EnemyState.IDLE;

			_visualController.Refresh(_state);
		}

		#endregion

		#region Health system

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

			Debug.LogError($"Enemy <color=yellow>{name}</color> was destroyed");

			RefreshDeathState();
		}

		#endregion
	}
}