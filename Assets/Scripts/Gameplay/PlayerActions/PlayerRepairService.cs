/*
 * Date: October 1st, 2023
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;
using System;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class PlayerRepairService : MonoBehaviour
    {
		#region Events

		public Action OnDetectTarget;
		public Action OnUndetectTarget;

		#endregion

		#region Inspector

		[SerializeField] private RepairObject[] _repairObjects = default;

		#endregion

		#region Private properties

		private float _speedRepairing = default;
		private const string _tag = "Repair";
		private RepairObject _target = default;
		private bool _targetDetected = false;
		private Action _onStart = default;
		private Action _onStop = default;
		private Action _onComplete = default;
		private RepairPartData _currentPart = null;
		private int _lastDetectedTarget = -1;
		private bool _isRepairing = false;
		private float _expiration = default;
		private int _objectId = -1;

		#endregion

		#region Unity events

		private void Awake()
		{
			PlayerProgressEvents.OnStartRepairing += HandleStartRepairing;
			PlayerProgressEvents.OnSpeedUpRepairing += HandleSpeedUpRepairing;
		}

		private void OnDestroy()
		{
			PlayerProgressEvents.OnStartRepairing -= HandleStartRepairing;
			PlayerProgressEvents.OnSpeedUpRepairing -= HandleSpeedUpRepairing;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<RepairObject>(out var repairObject)) return;

			DetectTarget(repairObject);
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<RepairObject>(out var repairObject)) return;

			UndetectTarget(repairObject);
		}

		private void Update()
		{
			if (!_isRepairing) return;

			ProcessRepairingProgress();
		}

		#endregion

		#region Public methods

		public void Init(Action onStart, Action onStop, Action onComplete)
		{
			_onStart = onStart;
			_onStop = onStop;
			_onComplete = onComplete;

			var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();

			progressDataService.Repair_Setup(_repairObjects);
		}

		public void Teardown()
		{
			_onStart = null;
			_onStop = null;
			_onComplete = null;
		}

		public void SetupSpeed(float speed)
		{
			_speedRepairing = speed;
		}

		public bool CanPickup()
		{
			return _currentPart == null;
		}

		public void PickupPart(RepairPartData data)
		{
			_currentPart = data;

			if (_canDebug) DebugPickupPart();

			UI_GameplayEvents.OnPickupedRepairPart?.Invoke(_currentPart);
		}

		#endregion

		#region Private methods

		private void DetectTarget(RepairObject target)
		{
			if (target.Id == _lastDetectedTarget) return;

			_lastDetectedTarget = target.Id;

			target.ShowIndicator();

			_target = target;

			_targetDetected = true;

			if (_canDebug) DebugDetection();

			OnDetectTarget?.Invoke();
		}

		private void UndetectTarget(RepairObject target)
		{
			_lastDetectedTarget = -1;

			if (target.WasCompleted) return;

			target.HideIndicator();

			_targetDetected = false;

			if (_canDebug) DebugUndetection(target);

			_target = null;

			OnUndetectTarget?.Invoke();
		}

		private void HandleStartRepairing(PlayerProgressEvents.RepairStartActionArgs args)
		{
			_objectId = args.id;
			_expiration = args.expiration;
			_isRepairing = true;
		}

		private void CompleteRepairing()
		{
			if (_canDebug) DebugRepairingCompletion();

			var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();
			progressDataService.Repair_CompleteProcess(_objectId);

			_isRepairing = false;
			_objectId = -1;
			_expiration = 0;
		}

		private void HandleSpeedUpRepairing(PlayerProgressEvents.RepairSpeedUpArgs args)
		{
			_objectId = args.id;
			_expiration = args.expiration;
			_isRepairing = true;
		}

		private void ProcessRepairingProgress()
		{
			if (_canDebug) DebugRepairingProgress();

			var remainingTime = _expiration - Time.time;

			if (remainingTime > 0) return;

			CompleteRepairing();
		}

		#endregion

		#region Debug

		private bool _canDebug = false;

		private void DebugDetection()
		{
			Debug.LogError($"Repair object detected: <color=green>{_target.name}</color>");
		}

		private void DebugUndetection(RepairObject target)
		{
			Debug.LogError($"Repair object stop detection: <color=red>{target.name}</color>");
		}

		private void DebugPickupPart()
		{
			Debug.LogError($"Pick part <color=cyan>{_currentPart.Id}</color>");
		}

		private void DebugRepairingProgress()
		{
			Debug.LogError($"Repairing progress -> remaining time: <color=cyan>{_expiration - Time.time}</color>");
		}

		private void DebugRepairingCompletion()
		{
			Debug.LogError($"Repairing progress -> <color=green>COMPLETED</color>");
		}

		#endregion
	}
}