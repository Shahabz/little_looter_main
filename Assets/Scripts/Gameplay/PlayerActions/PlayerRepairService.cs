/*
 * Date: October 1st, 2023
 * Author: Peche
 */

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

		private PlayerEntryPoint _entryPoint = default;
		private float _speedRepairing = default;
		private const string _tag = "Repair";
		private RepairObject _target = default;
		private bool _targetDetected = false;
		private Action _onStart = default;
		private Action _onStop = default;
		private Action _onComplete = default;
		private RepairPartData _currentPart = null;
		private int _lastDetectedTarget = -1;

		#endregion

		#region Unity events

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

		#endregion

		#region Public methods

		public void Init(PlayerEntryPoint entryPoint, Action onStart, Action onStop, Action onComplete)
		{
			_entryPoint = entryPoint;
			_onStart = onStart;
			_onStop = onStop;
			_onComplete = onComplete;

			_entryPoint.SetupRepairObjects(_repairObjects);
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

		public void CheckInput(StarterAssets.StarterAssetsInputs input)
		{
			// Check if there is a detected target
			if (!_targetDetected) return;

			// Check if repair input state is the same as the current target repairing state
			if (input.repair == _target.IsRepairing) return;

			// Check if it should start repairing
			if (input.repair)
			{
				StartRepairing();
				return;
			}

			// Else stop current target repairing state
			StopRepairing();
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

		private void StartRepairing()
		{
			_target.StartProcess(_currentPart, _speedRepairing, CompleteRepairing);

			_onStart?.Invoke();
		}

		private void StopRepairing()
		{
			if (!_targetDetected) return;

			_target.StopProcess();

			_onStop?.Invoke();
		}

		private void DetectTarget(RepairObject target)
		{
			if (target.Id == _lastDetectedTarget) return;

			_lastDetectedTarget = target.Id;

			if (_currentPart != null)
			{
				_entryPoint.AddPartsToRepairObject(target.Data.Id, _currentPart.Id, 1);

				UI_GameplayEvents.OnConsumedRepairPart?.Invoke(_currentPart);

				_currentPart = null;
			}

			target.ShowIndicator();

			//var (index, targetProgressData) = _entryPoint.ProgressData.GetRepairObjectProgressData(target.Data.Id);

			//target.RefreshState(targetProgressData);

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

			if (target.IsRepairing)
			{
				target.StopProcess();

				_onStop?.Invoke();
			}

			_target = null;

			OnUndetectTarget?.Invoke();
		}

		private void CompleteRepairing()
		{
			_onComplete?.Invoke();

			OnUndetectTarget?.Invoke();

			UI_GameplayEvents.OnConsumedRepairPart?.Invoke(_currentPart);

			_currentPart = null;
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

		#endregion
	}
}