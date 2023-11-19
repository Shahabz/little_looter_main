/*
 * Date: October 28th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using LittleLooters.Gameplay.UI;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

namespace LittleLooters.Gameplay
{
	/// <summary>
	/// Detects destructible objects near to the player.
	/// When any object is detected, then it is informed and the player's melee destruction action starts.
	/// When there is no other target detected or all targets were destroyed, then player's melee destruction action stops.
	/// </summary>
    public class PlayerDestructionService : MonoBehaviour
    {
		#region Inspector

		[SerializeField] PlayerEntryPoint _entryPoint = default;
		[SerializeField] private RuntimeAnimatorController _assaultAnimatorController = default;
		[SerializeField] private RuntimeAnimatorController _meleeAnimatorController = default;
		[SerializeField] private float _attackRate = 1.0f;
		[SerializeField] private float _radiusDetection = default;
		[SerializeField] private LayerMask _layer = default;
		[SerializeField] private float _angleFieldOfView = default;

		#endregion

		#region Private properties

		private bool _isPlayerAiming = false;
		private int _damage = -1;	// This value comes from player's melee weapon data
		private int _level = -1;	// This value comes from player's melee weapon data
		private float _delayToStartProcessing = 0.5f;
		private const string _tag = "Destructible";
		private ThirdPersonController _controller = default;
		private VisualCharacterController _visualController = default;
		private List<DestructibleResourceObject> _targets = default;
		private bool _isInProgress = false;
		private float _nextAttackRemainingTime = 0;
		private bool _playerIsDead = false;
		private bool _checkStopMoving = false;
		private DestructibleResourceObject _lookingTarget = default;
		private Collider[] _hits = default;
		private int _lastNotEnabledObject = -1;
		private float _lastNotEnabledDelay = 10;
		private float _remainingLastNotEnabledDelay = 0;

		#endregion

		#region Unity events

		private void Awake()
		{
			PlayerAimingAssistance.OnStartAiming += StartAiming;
			PlayerAimingAssistance.OnStopAiming += StopAiming;

			_hits = new Collider[100];

			_targets = new List<DestructibleResourceObject>();

			_controller = GetComponent<ThirdPersonController>();

			_visualController = GetComponent<VisualCharacterController>();

			if (!TryGetComponent<PlayerHealth>(out var health)) return;

			health.OnDead += PlayerDeath;

			DestructibleResourceEvents.OnGrantRewardsByDamage += GrantRewardsByDamage;
		}

		private void OnDestroy()
		{
			DestructibleResourceEvents.OnGrantRewardsByDamage -= GrantRewardsByDamage;
			
			PlayerAimingAssistance.OnStartAiming -= StartAiming;
			PlayerAimingAssistance.OnStopAiming -= StopAiming;

			if (!TryGetComponent<PlayerHealth>(out var health)) return;

			health.OnDead -= PlayerDeath;
		}

		private void Update()
		{
			if (_playerIsDead) return;

			CheckNotEnabledObject(Time.deltaTime);

			if (!CanProcess()) return;

			if (!_isInProgress)
			{
				CheckTargetsAround();

				return;
			}

			ProcessCheck(Time.deltaTime);
		}

		/*private void OnTriggerEnter(Collider other)
		{
			if (_playerIsDead) return;

			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<DestructibleResourceObject>(out var destructible)) return;

			DetectTarget(destructible);
		}
		*/

		/*private void OnTriggerExit(Collider other)
		{
			if (_playerIsDead) return;

			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<DestructibleResourceObject>(out var destructible)) return;

			UndetectTarget(destructible);
		}
		*/

		#endregion

		#region Private methods

		private bool CanProcess()
		{
			if (_isPlayerAiming) return false;

			return true;
		}

		private void DetectTarget(DestructibleResourceObject target)
		{
			/*
			if (!CanProcess()) return;

			if (target.IsDead) return;

			target.Detected();

			_targets.Add(target);

			if (_isInProgress) return;

			Invoke(nameof(StartProcessing), _delayToStartProcessing);
			*/
		}

		private void UndetectTarget(DestructibleResourceObject target)
		{
			/*
			if (!CanProcess()) return;

			target.Undetected();

			_targets.Remove(target);

			if (_targets.Count > 0) return;

			StopProcessing();
			*/
		}

		private void ProcessCheck(float deltaTime)
		{
			if (!CanProcess()) return;

			if (_controller.IsMoving())
			{
				StopByPlayerMovement();
				return;
			}

			_nextAttackRemainingTime -= deltaTime;

			if (_nextAttackRemainingTime > 0) return;

			_nextAttackRemainingTime = _attackRate;

			_visualController.ApplyMelee();
		}

		private void StopProcessing()
		{
			if (_canDebug) DebugStopProcessing();

			StopAllTargets();

			_controller.StopMeleeDestructionInteraction();

			if (!_animationStarted)
			{
				_visualController.OverrideAnimatorController(_assaultAnimatorController);
				_visualController.SetAssaultWeapon();
				_visualController.EnableRig();
			}

			_isInProgress = false;

			_nextAttackRemainingTime = 0;

			_checkStopMoving = false;
		}

		private void PlayerDeath()
		{
			_playerIsDead = true;

			StopProcessing();
		}

		private void StartProcessing()
		{
			if (_canDebug) DebugStartProcessing();

			_isInProgress = true;

			_lookingTarget = GetNearestTarget();

			_controller.StartMeleeDestructionInteraction();

			_controller.LookAtMeleeTarget(_lookingTarget.transform);

			_visualController.OverrideAnimatorController(_meleeAnimatorController);
			_visualController.SetMeleeWeapon();
			_visualController.DisableRig();
		}

		private void GrantRewardsByDamage(int resourceId, int amountReward)
		{
			_entryPoint.GrantResourceByDestructionDamage(resourceId, amountReward);

			UI.UI_ResourcesAnimation.OnAnimate?.Invoke(resourceId, amountReward);
		}

		private DestructibleResourceObject GetNearestTarget()
		{
			return _targets[0];
		}

		private void StopByPlayerMovement()
		{
			if (_canDebug) DebugStopByMovement();

			CancelProcessing();

			GetTargetsOut();
		}

		private void CancelProcessing()
		{
			StopAllTargets();

			_targets.Clear();

			_controller.StopMeleeDestructionInteraction();

			_animationStarted = false;

			_visualController.OverrideAnimatorController(_assaultAnimatorController);
			_visualController.SetAssaultWeapon();
			_visualController.EnableRig();

			_isInProgress = false;

			_nextAttackRemainingTime = 0;

			_checkStopMoving = false;
		}

		private void StartAiming()
		{
			_isPlayerAiming = true;

			//if (!_isInProgress && !_animationStarted) return;

			CancelProcessing();
		}

		private void StopAiming()
		{
			_isPlayerAiming = false;
		}

		private void StopAllTargets()
		{
			for (int i = 0; i < _targets.Count; i++)
			{
				var target = _targets[i];

				target.Undetected();
			}
		}

		private void CheckNotEnabledObject(float deltaTime)
		{
			if (_remainingLastNotEnabledDelay < 0) return;

			_remainingLastNotEnabledDelay -= deltaTime;

			if (_remainingLastNotEnabledDelay > 0) return;

			_remainingLastNotEnabledDelay = 0;
			_lastNotEnabledObject = -1;
		}

		#endregion

		#region Detection methods

		private void CheckTargetsAround()
		{
			GetTargetsOut();

			_targets.Clear();

			// Calculate possible targets inside a circle
			var targetsAround = GetTargetsAround();

			if (targetsAround.Count == 0) return;

			var playerPosition = transform.position;
			var playerForward = transform.forward;
			var targetsInsideFoV = new List<DestructibleResourceObject>();

			// For each possible target check if there is at least one in the cone vision
			for (int i = 0; i < targetsAround.Count; i++)
			{
				var possibleTarget = targetsAround[i];

				var directionToTarget = possibleTarget.transform.position - playerPosition;

				var angle = GetAngle(directionToTarget, playerForward);

				if (angle > _angleFieldOfView) continue;

				targetsInsideFoV.Add(possibleTarget);
			}

			if (targetsInsideFoV.Count > 0)
			{
				_targets = targetsInsideFoV;
			}
			else if (targetsAround.Count > 0)
			{
				_targets = targetsAround;
			}
			else
			{
				return;
			}

			MarkAsDetected();

			// If player is moving, skip process
			if (_controller.IsMoving()) return;

			var nearest = GetNearestTarget();

			if (_lastNotEnabledObject == nearest.Id) return;

			var currentToolLevel = _entryPoint.ProgressData.meleeData.level;

			if (nearest.LevelRequired > currentToolLevel)
			{
				_lastNotEnabledObject = nearest.Id;
				_remainingLastNotEnabledDelay = _lastNotEnabledDelay;
			}

			// Start process
			StartProcessing();
		}

		private void MarkAsDetected()
		{
			for (int i = 0; i < _targets.Count; i++)
			{
				var target = _targets[i];

				target.Detected();
			}
		}

		private void MarkAsNoDetected(DestructibleResourceObject[] targets)
		{
			for (int i = 0; i < targets.Length; i++)
			{
				var target = targets[i];

				target.Undetected();
			}
		}

		private List<DestructibleResourceObject> GetTargetsAround()
		{
			var currentPosition = transform.position;

			var amount = Physics.OverlapSphereNonAlloc(currentPosition, _radiusDetection, _hits, _layer);

			var result = new List<DestructibleResourceObject>(amount);

			for (int i = 0; i < amount; i++)
			{
				var hit = _hits[i];

				var target = hit.gameObject.GetComponent<DestructibleResourceObject>();

				if (target.IsDead) continue;

				result.Add(target);
			}

			return result;
		}

		private float GetAngle(Vector3 direction, Vector3 forward)
		{
			var angleToTarget = Mathf.Abs(Vector3.Angle(direction, forward));

			return angleToTarget;
		}

		private void GetTargetsOut()
		{
			var currentPosition = transform.position;

			var amount = Physics.OverlapSphereNonAlloc(currentPosition, _radiusDetection + 10, _hits, _layer);

			var result = new List<DestructibleResourceObject>(amount);

			for (int i = 0; i < amount; i++)
			{
				var hit = _hits[i];

				var target = hit.gameObject.GetComponent<DestructibleResourceObject>();

				result.Add(target);
			}

			MarkAsNoDetected(result.ToArray());
		}

		#endregion

		#region Animation events

		private bool _animationStarted = false;

		public void EventAnimationStart()
		{
			_animationStarted = true;

			if (_canDebug) DebugAnimationStart();
		}

		public void EventAnimationImpact()
		{
			if (_canDebug) DebugAnimationImpact();

			var destroyed = false;

			for (int i = 0; i < _targets.Count; i++)
			{
				var target = _targets[i];

				if (target.IsDead) continue;

				if (target.LevelRequired > _entryPoint.ProgressData.meleeData.level)
				{
					destroyed = true;

					UI_TextDamagePanel.OnAnimateLevelRequired?.Invoke(target.transform.position, target.LevelRequired);

					target.AnimateDamage();

					_targets.Remove(target);

					target.Undetected();

					continue;
				}
				else
				{
					target.TakeDamage(_entryPoint.ProgressData.meleeData.damage);
				}

				if (!target.IsDead) continue;

				destroyed = true;

				_targets.Remove(target);

				target.Undetected();
			}

			// Check if all targets were destroyed
			if (_targets.Count > 0)
			{
				if (destroyed && !_targets.Contains(_lookingTarget))
				{
					_lookingTarget = GetNearestTarget();

					_controller.LookAtMeleeTarget(_lookingTarget.transform);
				}

				return;
			}

			StopProcessing();
		}

		public void EventAnimationEnd()
		{
			if (_canDebug) DebugAnimationEnd();

			_animationStarted = false;

			if (_targets.Count > 0) return;

			_visualController.OverrideAnimatorController(_assaultAnimatorController);
			_visualController.SetAssaultWeapon();
			_visualController.EnableRig();
		}

		#endregion

		#region Debug

		private bool _canDebug = false;

		private void DebugStartProcessing()
		{
			Debug.LogError("<color=green>START</color> processing");
		}

		private void DebugStopProcessing()
		{
			Debug.LogError("<color=red>STOP</color> processing");
		}

		private void DebugAnimationStart()
		{
			Debug.LogError("Animation <color=green>STARTED</color>");
		}

		private void DebugAnimationImpact()
		{
			Debug.LogError("Animation <color=orange>IMPACT</color>");
		}

		private void DebugAnimationEnd()
		{
			Debug.LogError("Animation <color=red>ENDED</color>");
		}

		private void DebugStopByMovement()
		{
			Debug.LogError("Stop processing by <color=magenta>PLAYER's MOVEMENT</color>");
		}

		#endregion
	}
}