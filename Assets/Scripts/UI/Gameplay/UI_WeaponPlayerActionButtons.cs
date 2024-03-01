/*
 * Date: January 14th, 2024
 * Author: Peche
 */

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

        #region Unity events

        private void Awake()
        {
            LevelEnemies.OnStartDetection += HandleStartEnemiesDetection;
            //LevelEnemies.OnStopDetection += HandleStopEnemiesDetection;
        }

        private void Start()
        {
            HideButtons();
        }

        private void OnDestroy()
        {
            LevelEnemies.OnStartDetection -= HandleStartEnemiesDetection;
            //LevelEnemies.OnStopDetection -= HandleStopEnemiesDetection;
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

            _firstTime = false;

            ShowButtons();
        }

        #endregion
    }
}