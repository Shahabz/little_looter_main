/*
 * Date: January 14th, 2024
 * Author: Peche
 */

using System;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_WeaponPlayerActionButtons : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private GameObject _fireButton = default;
        [SerializeField] private GameObject _rollingButton = default;

        #endregion

        private bool _firstTime = true;
        private bool _autofire = false;

        #region Unity events

        private void Awake()
        {
            LevelEnemies.OnStartDetection += HandleStartEnemiesDetection;
            //LevelEnemies.OnStopDetection += HandleStopEnemiesDetection;
            UI_GameplayEvents.OnAutofireChangedByCheat += HandleAutofireChangedByCheats;
        }

        private void Start()
        {
            HideButtons();
        }

        private void OnDestroy()
        {
            LevelEnemies.OnStartDetection -= HandleStartEnemiesDetection;
            //LevelEnemies.OnStopDetection -= HandleStopEnemiesDetection;
            UI_GameplayEvents.OnAutofireChangedByCheat -= HandleAutofireChangedByCheats;
        }

		#endregion

		#region Private methods

		private void HideButtons()
        {
            _fireButton.SetActive(false);
            _rollingButton.SetActive(false);
        }

        private void ShowButtons()
        {
            _fireButton.SetActive(true);
            _rollingButton.SetActive(true);
        }

        private void HandleStopEnemiesDetection()
        {
            HideButtons();
        }

        private void HandleStartEnemiesDetection()
        {
            if (!_firstTime) return;

            if (_autofire) return;

            _firstTime = false;

            ShowButtons();
        }

        private void HandleAutofireChangedByCheats(UI_GameplayEvents.AutofireByCheatArgs args)
        {
            _autofire = args.enabled;

            if (args.enabled)
			{
                HideButtons();
                return;
			}

            ShowButtons();
        }

        #endregion
    }
}