/*
 * Date: September 16th, 2023
 * Author: Peche
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class LevelEnemies : MonoBehaviour
    {
		#region Events

		public static System.Action OnStartDetection;
		public static System.Action OnStopDetection;
		public static System.Action OnTargetInsideRange;
		public static System.Action OnTargetOutsideRange;

		#endregion

		[SerializeField] private EnemyController[] _entities = default;
		[SerializeField] private Transform _player = default;
		[SerializeField] private Camera _camera = default;

		public EnemyController[] Entities => _entities;

		[SerializeField] private bool _enabled = false; 
		private int _detectedId = -1;
		private EnemyController _detected = default;
		private bool _detectionStarted = false;
		private int _enemyId = 0;
		private List<EnemyController> _enemiesAlive = default;

		private void Start()
		{
			UI_GameplayEvents.OnStartGame += HandleStartGame;

			_enemiesAlive = new List<EnemyController>(50);
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
				if (_detectedId != -1) OnTargetOutsideRange?.Invoke();

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

				OnTargetInsideRange?.Invoke();
			}

			if (!previousDetection) return;

			previousDetected.MarkAsNonDetected();
		}

		public void StopDetection()
		{
			if (!_detectionStarted) return;

			if (_canDebug) DebugStopDetection();

			ClearTargetDetection();

			RemoveDeadEntities();

			_detectionStarted = false;

			OnStopDetection?.Invoke();
		}

		public void AddNewEnemy(EnemyController enemy)
		{
			var enemies = _entities.ToList();

			enemies.Add(enemy);

			enemy.name += $"_{_enemyId}";

			enemy.SetTarget(_player);
			enemy.SetCamera(_camera);
			enemy.Initialization(_enemyId);

			_entities = enemies.ToArray();

			_enemyId++;
		}

		private void InitEntities()
		{
			_enemyId = 0;

			for (int i = 0; i < _entities.Length; i++)
			{
				var entity = _entities[i];

				entity.Initialization(_enemyId);

				_enemyId++;
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

		private void RemoveDeadEntities()
		{
			_enemiesAlive.Clear();

			for (int i = 0; i < _entities.Length; i++)
			{
				var entity = _entities[i];

				if (entity.IsDead) continue;

				_enemiesAlive.Add(entity);
			}

			_entities = _enemiesAlive.ToArray();
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