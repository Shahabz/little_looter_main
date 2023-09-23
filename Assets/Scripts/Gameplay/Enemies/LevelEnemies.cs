/*
 * Date: September 16th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class LevelEnemies : MonoBehaviour
    {
        [SerializeField] private EnemyController[] _entities = default;

		private void Start()
		{
			InitEntities();
		}

		private void OnDestroy()
		{
			TeardownEntities();
		}

		private void InitEntities()
		{
			for (int i = 0; i < _entities.Length; i++)
			{
				var entity = _entities[i];

				entity.Initialization();
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