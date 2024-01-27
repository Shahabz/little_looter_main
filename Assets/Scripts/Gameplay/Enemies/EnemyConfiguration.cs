/*
 * Date: January 27th, 2024
 * Author: Peche
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    [System.Serializable]
    public struct EnemyLevelConfiguration
	{
        public int level;
        public int hp;
        public int minDamage;
        public int maxDamage;
    }

    [CreateAssetMenu(fileName = "EnemyConfiguration", menuName = "ScriptableObjects/Gameplay/Enemies/Configuration", order = 1)]
    public class EnemyConfiguration : ScriptableObject
    {
        public int id;
        public EnemyLevelConfiguration[] levelsConfiguration;

        private Dictionary<int, EnemyLevelConfiguration> _internalLevels = default;
        private bool _levelsWereInitialized = false;

		private void OnEnable()
		{
            _levelsWereInitialized = false;
		}

		public EnemyLevelConfiguration GetLevelConfiguration(int level)
		{
            if (!_levelsWereInitialized)
			{
                InitializeLevels();
			}

            if (!_internalLevels.TryGetValue(level, out var config)) return new EnemyLevelConfiguration();

            return config;
		}

        private void InitializeLevels()
		{
            _internalLevels = levelsConfiguration.ToDictionary(key => key.level, value => value);

            _levelsWereInitialized = true;
		}
    }
}