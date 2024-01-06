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

		private void Start()
		{
			InitEntities();
		}

		private void OnDestroy()
		{
			TeardownEntities();
		}

		private void Update()
		{
			if (!_enabled) return;

			RefreshEntities();
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

		public void StartDetection(EnemyController target)
		{
			if (_detectedId == target.Id) return;

			var previousDetection = _detectedId != -1;
			var previousDetected = _detected;

			if (!previousDetection) OnStartDetection?.Invoke();

			_detected = target;
			_detectedId = target.Id;
			_detected.MarkAsDetected();

			if (!previousDetection) return;

			previousDetected.MarkAsNonDetected();
		}

		public void StopDetection()
		{
			if (_detectedId == -1) return;

			_detected.MarkAsNonDetected();

			_detected = null;
			_detectedId = -1;

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
	}
}