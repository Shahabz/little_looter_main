/*
 * Date: September 16th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class LevelEnemies : MonoBehaviour
    {
		#region Events

		public static System.Action OnStartDetection;
		public static System.Action OnStopDetection;

		#endregion

		[SerializeField] private EnemyController[] _entities = default;

		public EnemyController[] Entities => _entities;

		[SerializeField] private bool _enabled = false; 
		private int _detectedId = -1;
		private EnemyController _detected = default;
		private bool _detectionStarted = false;

		private void Start()
		{
			UI_GameplayEvents.OnStartGame += HandleStartGame;
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnStartGame -= HandleStartGame;

			TeardownEntities();
		}

		private void Update()
		{
			if (!_enabled) return;

			RefreshEntities();
		}

		private void HandleStartGame()
		{
			InitEntities();
		}

		private void RefreshEntities()
		{
			for (int i = 0; i < _entities.Length; i++)
			{
				var entity = _entities[i];

				if (entity.IsDead) continue;

				entity.Tick(Time.deltaTime);
			}
		}

		public void StartDetection(EnemyController target, bool isTargetInsideRadius)
		{
			if (isTargetInsideRadius && _detectedId == target.Id) return;

			if (!isTargetInsideRadius)
			{
				ClearTargetDetection();
			}

			var previousDetection = _detectedId != -1;
			var previousDetected = _detected;

			if (!previousDetection)
			{
				_detectionStarted = true;

				OnStartDetection?.Invoke();
			}

			if (_canDebug) DebugStartDetection(isTargetInsideRadius);

			if (isTargetInsideRadius)
			{
				_detected = target;
				_detectedId = target.Id;
				_detected.MarkAsDetected();
			}

			if (!previousDetection) return;

			previousDetected.MarkAsNonDetected();
		}

		public void StopDetection()
		{
			if (!_detectionStarted) return;

			if (_canDebug) DebugStopDetection();

			ClearTargetDetection();

			_detectionStarted = false;

			OnStopDetection?.Invoke();
		}

		private void InitEntities()
		{
			for (int i = 0; i < _entities.Length; i++)
			{
				var entity = _entities[i];

				entity.Initialization(i);
			}
		}

		private void TeardownEntities()
		{
			for (int i = 0; i < _entities.Length; i++)
			{
				var entity = _entities[i];

				entity.Teardown();
			}
		}

		private void ClearTargetDetection()
		{
			if (_detectedId == -1) return;

			_detected.MarkAsNonDetected();

			_detected = null;
			_detectedId = -1;
		}

		#region Debug

		private bool _canDebug = false;

		private void DebugStartDetection(bool isTargetInsideRadius)
		{
			Debug.LogError($"LevelEnemies::<color=green>StartDetection</color> -> is target inside radius: <color=yellow>{isTargetInsideRadius}</color>");
		}

		private void DebugStopDetection()
		{
			Debug.LogError($"LevelEnemies::<color=red>StopDetection</color>");
		}

		#endregion
	}
}