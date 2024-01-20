/*
 * Date: January 20th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Global.ServiceLocator
{
    public class Bootstrapper : MonoBehaviour
    {
		#region Inspector

        [SerializeField] private GameConfigurationService _gameConfigurationService = default;
		[SerializeField] private PlayerProgressDataService _playerProgressService = default;

        #endregion

        #region Unity events

        private void Awake()
        {
            // Initialize default service locator.
            ServiceLocator.Initialize();

            // Register all your services next.
            ServiceLocator.Current.Register(_gameConfigurationService);
            ServiceLocator.Current.Register(_playerProgressService);
        }

		#endregion
	}
}