/*
 * Date: October 28th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using LittleLooters.Gameplay.UI;
using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;
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

		[SerializeField] private RuntimeAnimatorController _assaultAnimatorController = default;
		[SerializeField] private RuntimeAnimatorController _meleeAnimatorController = default;
		[SerializeField] private float _attackRate = 1.0f;
		[SerializeField] private float _radiusDetection = default;
		[SerializeField] private float _radiusDetectionOffset = 5;
		[SerializeField] private LayerMask _layer = default;
		[SerializeField] private float _angleFieldOfView = default;
		[SerializeField] private GameObject _extraDamageVx = default;

		#endregion

		#region Private properties

		private bool _isEnabled = true;
		private bool _isPlayerAiming = false;
		private bool _isPlayerRolling = false;
		private int _damage = -1;   // This value comes from player's melee weapon data
		private int _level = -1;    // This value comes from player's melee weapon data
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
		private Transform _transform = default;
		private float _timeSinceLastDetection = 0;
		private float _timeBetweenDetections = 0.5f;
		private List<DestructibleResourceObject> _targetsInsideFoV = default;
		private List<DestructibleResourceObject> _targetsAround = default;
		private List<DestructibleResourceObject> _targetsOut = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			PlayerAimingAssistance.OnStartAiming += StartAiming;
			PlayerAimingAssistance.OnStopAiming += StopAiming;

			PlayerProgressEvents.OnToolDamageIncreaseStarted += HandleStartToolDamageIncrease;
			PlayerProgressEvents.OnToolDamageIncreaseCompleted += HandleStopToolDamageIncrease;

			UI_GameplayEvents.OnDestructionDetectionChangedByCheat += HandleDestructionStatusChangedByCheat;

			_transform = transform;

			_hits = new Collider[100];

			_targets = new List<DestructibleResourceObject>();

			_targetsInsideFoV = new List<DestructibleResourceObject>();

			_targetsAround = new List<DestructibleResourceObject>();

			_targetsOut = new List<DestructibleResourceObject>();

			_controller = GetComponent<ThirdPersonController>();

			_controller.OnStartRolling += HandleOnStartRolling;
			_controller.OnStopRolling += HandleOnStopRolling;

			_visualController = GetComponent<VisualCharacterController>();

			HideExtraDamageVfx();

			DestructibleResourceEvents.OnGrantRewardsByDamage += GrantRewardsByDamage;

			if (!TryGetComponent<PlayerHealth>(out var health)) return;

			health.OnDead += PlayerDeath;
		}

		private void OnDestroy()
		{
			PlayerAimingAssistance.OnStartAiming -= StartAiming;
			PlayerAimingAssistance.OnStopAiming -= StopAiming;

			PlayerProgressEvents.OnToolDamageIncreaseStarted -= HandleStartToolDamageIncrease;
			PlayerProgressEvents.OnToolDamageIncreaseCompleted -= HandleStopToolDamageIncrease;

			DestructibleResourceEvents.OnGrantRewardsByDamage -= GrantRewardsByDamage;

			UI_GameplayEvents.OnDestructionDetectionChangedByCheat -= HandleDestructionStatusChangedByCheat;

			if (_controller != null)
			{
				_controller.OnStartRolling -= HandleOnStartRolling;
				_controller.OnStopRolling -= HandleOnStopRolling;
			}

			if (!TryGetComponent<PlayerHealth>(out var health)) return;

			health.OnDead -= PlayerDeath;
		}

		private void Update()
		{
			// TODO: add delay between executions

			if (!_isEnabled) return;

			if (_playerIsDead) return;

			CheckToolExtraDamageProgress();

			CheckNotEnabledObject(Time.deltaTime);

			if (!CanProcess()) return;

			if (_controller.IsMoving())
			{
				if (_isInProgress)
				{
					StopByPlayerMovement();
				}

				return;
			}

			if (_controller.IsRolling)
			{
				if (_isInProgress)
				{
					StopByPlayerMovement();
				}

				return;
			}

			if (!_isInProgress)
			{
				CheckTargetsAround();

				return;
			}

			ProcessCheck(Time.deltaTime);
		}

		#endregion

		#region Private methods

		private bool CanProcess()
		{
			if (_isPlayerAiming) return false;

			if (_isPlayerRolling) return false;

			return true;
		}

		private void ProcessCheck(float deltaTime)
		{
			_nextAttackRemainingTime -= deltaTime;

			if (_nextAttackRemainingTime > 0) return;

			_nextAttackRemainingTime = _attackRate;

			_visualController.ApplyMelee();
		}

		private void StopProcessing()
		{
			if (_canDebug) DebugStopProcessing();

			StopAllTargets();

			_targets.Clear();

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
			if (_canDebug) DebugStartProcessing(_targets.Count);

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
			var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();

			progressDataService.Resources_GrantByDestructionDamage(resourceId, amountReward);

			UI_ResourcesAnimation.OnAnimate?.Invoke(resourceId, amountReward);
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

		private void HandleOnStartRolling()
		{
			_isPlayerRolling = true;
			CancelProcessing();
		}

		private void HandleOnStopRolling()
		{
			_isPlayerRolling = false;
		}

		private void HandleDestructionStatusChangedByCheat(bool status)
		{
			_isEnabled = status;

			if (_isEnabled) return;

			StopProcessing();
		}

		#endregion

		#region Tool extra damage

		private float _toolExtraDamageExpiration = 0;
		private bool _toolExtraDamageInProgress = false;

		private void HandleStartToolDamageIncrease(PlayerProgressEvents.ToolExtraDamageStartedArgs args)
		{
			_toolExtraDamageExpiration = args.expiration;

			_toolExtraDamageInProgress = true;

			ShowExtraDamageVfx();
		}

		private void HandleStopToolDamageIncrease()
		{
			HideExtraDamageVfx();
		}

		private void CheckToolExtraDamageProgress()
		{
			if (!_toolExtraDamageInProgress) return;

			if (_toolExtraDamageExpiration > Time.time) return;

			CompleteToolExtraDamage();
		}

		private void CompleteToolExtraDamage()
		{
			_toolExtraDamageExpiration = 0;

			_toolExtraDamageInProgress = false;

			var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();
			progressDataService.Tool_CompleteExtraDamage();
		}

		private void ShowExtraDamageVfx()
		{
			_extraDamageVx.SetActive(true);
		}

		private void HideExtraDamageVfx()
		{
			_extraDamageVx.SetActive(false);
		}

		#endregion

		#region Detection methods

		private void CheckTargetsAround()
		{
			GetTargetsOut();

			_targets.Clear();

			// Calculate possible targets inside a circle
			GetTargetsAround();

			if (_targetsAround.Count == 0) return;

			var playerPosition = _transform.position;
			var playerForward = _transform.forward;

			_targetsInsideFoV.Clear();

			// For each possible target check if there is at least one in the cone vision
			for (int i = 0; i < _targetsAround.Count; i++)
			{
				var possibleTarget = _targetsAround[i];

				var directionToTarget = possibleTarget.transform.position - playerPosition;

				var angle = GetAngle(directionToTarget, playerForward);

				if (angle > _angleFieldOfView) continue;

				if (_targetsInsideFoV.Contains(possibleTarget)) continue;

				_targetsInsideFoV.Add(possibleTarget);
			}

			if (_targetsInsideFoV.Count > 0)
			{
				_targets = _targetsInsideFoV;
			}
			else if (_targetsAround.Count > 0)
			{
				_targets = _targetsAround;
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

			var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();

			var currentToolLevel = progressDataService.ProgressData.toolData.level;

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

		private void GetTargetsAround()
		{
			var currentPosition = _transform.position;

			var amount = Physics.OverlapSphereNonAlloc(currentPosition, _radiusDetection, _hits, _layer);

			_targetsAround.Clear();

			for (int i = 0; i < amount; i++)
			{
				var hit = _hits[i];

				var target = hit.gameObject.GetComponent<DestructibleResourceObject>();

				if (target.IsDead) continue;

				if (_targetsAround.Contains(target)) continue;

				_targetsAround.Add(target);
			}
		}

		private float GetAngle(Vector3 direction, Vector3 forward)
		{
			var angleToTarget = Mathf.Abs(Vector3.Angle(direction, forward));

			return angleToTarget;
		}

		private void GetTargetsOut()
		{
			var currentPosition = _transform.position;

			var amount = Physics.OverlapSphereNonAlloc(currentPosition, _radiusDetection + _radiusDetectionOffset, _hits, _layer);

			_targetsOut.Clear();

			for (int i = 0; i < amount; i++)
			{
				var hit = _hits[i];

				var target = hit.gameObject.GetComponent<DestructibleResourceObject>();

				_targetsOut.Add(target);
			}

			MarkAsNoDetected(_targetsOut.ToArray());
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

			var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();
			var toolLevel = progressDataService.ProgressData.toolData.level;

			for (int i = 0; i < _targets.Count; i++)
			{
				var target = _targets[i];

				if (target.IsDead) continue;

				if (target.LevelRequired > toolLevel)
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
					target.TakeDamage(progressDataService.ProgressData.toolData.damage);
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

		private void DebugStartProcessing(int amount)
		{
			Debug.LogError($"<color=green>START</color> processing -> amount detected: <color=yellow>{amount}</color>");
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

		/*private void OnDrawGizmosSelected()
		{
#if UNITY_EDITOR
			// detection area
			UnityEditor.Handles.color = Color.red;
			UnityEditor.Handles.DrawWireDisc(transform.position, new Vector3(0, 1, 0), _radiusDetection);

			// detection out area
			UnityEditor.Handles.color = Color.yellow;
			UnityEditor.Handles.DrawWireDisc(transform.position, new Vector3(0, 1, 0), _radiusDetection + _radiusDetectionOffset);
#endif
		}*/

		#endregion
	}
}