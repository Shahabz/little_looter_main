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

		#endregion

		#region Private properties

		private int _damage = 5;    // TODO: this value should come from player's melee weapon data
		private int _level = 1;		// TODO: this value should come from player's melee weapon data
		private float _attackRate = 2.5f; // TODO: this value should come from player's melee weapon data
		private float _delayToStartProcessing = 0.5f; // TODO: this value should come from player's melee weapon data
		private const string _tag = "Destructible";
		private ThirdPersonController _controller = default;
		private VisualCharacterController _visualController = default;
		private List<DestructibleResourceObject> _targets = default;
		private bool _isInProgress = false;
		private float _nextAttackRemainingTime = 0;
		private bool _playerIsDead = false;
		private bool _checkStopMoving = false;
		private DestructibleResourceObject _lookingTarget = default;

		#endregion

		#region Unity events

		private void Awake()
		{
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

			if (!TryGetComponent<PlayerHealth>(out var health)) return;

			health.OnDead -= PlayerDeath;
		}

		private void Update()
		{
			if (_playerIsDead) return;

			if (_checkStopMoving)
			{
				_checkStopMoving = false;
				Invoke(nameof(StartProcessing), _delayToStartProcessing);
				return;
			}

			if (!_isInProgress) return;

			ProcessCheck(Time.deltaTime);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (_playerIsDead) return;

			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<DestructibleResourceObject>(out var destructible)) return;

			DetectTarget(destructible);
		}

		private void OnTriggerExit(Collider other)
		{
			if (_playerIsDead) return;

			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<DestructibleResourceObject>(out var destructible)) return;

			UndetectTarget(destructible);
		}

		#endregion

		#region Private methods

		private void DetectTarget(DestructibleResourceObject target)
		{
			if (target.IsDead) return;

			target.Detected();

			_targets.Add(target);

			if (_isInProgress) return;

			Invoke(nameof(StartProcessing), _delayToStartProcessing);
		}

		private void UndetectTarget(DestructibleResourceObject target)
		{
			target.Undetected();

			_targets.Remove(target);

			if (_targets.Count > 0) return;

			StopProcessing();
		}

		private void ProcessCheck(float deltaTime)
		{
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
			if (_controller.IsMoving())
			{
				_checkStopMoving = true;
				return;
			}

			if (_targets.Count == 0) return;

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

				if (target.LevelRequired > _level)
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
					target.TakeDamage(_damage);
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