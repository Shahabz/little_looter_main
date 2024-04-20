/*
 * Date: April 13th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class CheatEnemiesSpawner : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private float _radius = default;
		[SerializeField] private Transform _pivot = default;
		[SerializeField] private EnemyController _enemyPrefab = default;
		[SerializeField] private LevelEnemies _levelEnemies = default;
		[SerializeField] private Transform _parent = default;

		#endregion

		#region Private properties

		#endregion

		#region Unity events

		private void Awake()
		{
			UI_GameplayEvents.OnSpawnEnemiesByCheat += HandleSpawnEnemiesByCheat;
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnSpawnEnemiesByCheat -= HandleSpawnEnemiesByCheat;
		}

		#endregion

		#region Private methods

		private void HandleSpawnEnemiesByCheat(int amount)
		{
			SpawnEnemies(amount);
		}

		private void SpawnEnemies(int amount)
		{
			var startPosition = _pivot.position;

			// Loop to spawn objects
			for (int i = 0; i < amount; i++)
			{
				// Calculate random position within spawnRadius
				Vector3 randomPos = startPosition + UnityEngine.Random.insideUnitSphere * _radius;

				// Set the y position to 0 to ensure objects spawn on the same level
				randomPos.y = 0f;

				// Spawn the object at the random position
				var enemy = Instantiate(_enemyPrefab, randomPos, Quaternion.identity);
				enemy.name = $"enemy_cheat";
				enemy.transform.SetParent(_parent);

				_levelEnemies.AddNewEnemy(enemy);

				Debug.LogError($"Enemy '<color=magenta>{enemy.name}</color>'");
			}
		}

		#endregion
	}
}