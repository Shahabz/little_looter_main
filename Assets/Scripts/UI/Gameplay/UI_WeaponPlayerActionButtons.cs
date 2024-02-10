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
        [SerializeField] private GameObject _reloadButton = default;

        #endregion

        #region Unity events

        private void Awake()
        {
            LevelEnemies.OnStartDetection += HandleStartEnemiesDetection;
            LevelEnemies.OnStopDetection += HandleStopEnemiesDetection;
        }

        private void Start()
        {
            HideButtons();
        }

        private void OnDestroy()
        {
            LevelEnemies.OnStartDetection -= HandleStartEnemiesDetection;
            LevelEnemies.OnStopDetection -= HandleStopEnemiesDetection;
        }

        #endregion

        #region Private methods

        private void HideButtons()
        {
            //_fireButton.SetActive(false);
            //_reloadButton.SetActive(false);
        }

        private void ShowButtons()
        {
            //_fireButton.SetActive(true);
            //_reloadButton.SetActive(true);
        }

        private void HandleStopEnemiesDetection()
        {
            HideButtons();
        }

        private void HandleStartEnemiesDetection()
        {
            ShowButtons();
        }

        #endregion
    }
}