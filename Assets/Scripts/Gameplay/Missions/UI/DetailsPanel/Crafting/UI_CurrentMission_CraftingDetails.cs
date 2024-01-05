/*
 * Date: December 27th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_CurrentMission_CraftingDetails : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private PlayerCraftingService _playerCraftingService = default;
        [SerializeField] private GameObject _progressBar = default;
        [SerializeField] private Image _progressBarFill = default;
        [SerializeField] private TMPro.TextMeshProUGUI _progressBarText = default;
        [SerializeField] private GameObject _progressBarCompleted = default;

        #endregion

        #region Private properties

        private int _id = -1;
        private bool _inProgress = false;
        private float _duration = 0;
        private float _expiration = 0;

        #endregion

        #region Unity events

        private void OnEnable()
        {
            PlayerProgressEvents.OnCraftingAreaProcessStarted += HandleProcessStarted;
            PlayerProgressEvents.OnCraftingAreaProcessCompleted += HandleProcessCompleted;
            PlayerProgressEvents.OnCraftingAreaProcessSpeedUp += HandleProcessSpeedUp;
            PlayerProgressEvents.OnCraftingAreaProcessClaimed += HandleProcessClaimed;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void Update()
        {
            if (!_inProgress) return;

            RefreshProgress();
        }

        #endregion

        #region Public methods

        public void Setup(int areaId)
        {
            _id = areaId;

            ResetProgressBar();
        }

        #endregion

        #region Private methods

        private void UnsubscribeEvents()
        {
            PlayerProgressEvents.OnCraftingAreaProcessStarted -= HandleProcessStarted;
            PlayerProgressEvents.OnCraftingAreaProcessCompleted -= HandleProcessCompleted;
            PlayerProgressEvents.OnCraftingAreaProcessSpeedUp -= HandleProcessSpeedUp;
            PlayerProgressEvents.OnCraftingAreaProcessClaimed -= HandleProcessClaimed;
        }

        private void RefreshProgress()
        {
            var now = Time.time;
            var remainingTime = _expiration - now;

            var secs = remainingTime;
            var mins = Mathf.FloorToInt(secs / 60);

            secs = Mathf.CeilToInt(secs - mins * 60);

            _progressBarText.text = $"{mins:00}:{secs:00}";

            var progress = 1 - remainingTime / _duration;

            _progressBarFill.fillAmount = progress;
        }

        private void HandleProcessStarted(PlayerProgress_CraftingAreaData data)
        {
            _inProgress = true;

            var craftingArea = _playerCraftingService.GetConfigurationAreaData(data.id);

            _duration = craftingArea.DurationByUnitInSecs * data.amount;

            _expiration = data.expiration;

            _progressBarCompleted.SetActive(false);

            _progressBar.SetActive(true);

            RefreshProgress();
        }

        private void HandleProcessCompleted(PlayerProgress_CraftingAreaData data)
        {
            if (_id != data.id) return;

            _inProgress = false;

            CompleteProgressBar();
        }

        private void HandleProcessSpeedUp(PlayerProgress_CraftingAreaData data)
        {
            if (_id != data.id) return;

            if (!_inProgress) return;

            _expiration = data.expiration;

            var now = Time.time;

            if (_expiration <= now)
			{
                _inProgress = false;

                _expiration = 0;

                _duration = 0;

                CompleteProgressBar();

                return;
			}

            RefreshProgress();
        }

        private void HandleProcessClaimed(PlayerProgress_CraftingAreaData data)
        {
            _inProgress = false;

            _expiration = 0;

            _duration = 0;

            ResetProgressBar();
        }

        private void ResetProgressBar()
		{
            _progressBarText.text = "NON IN PROGRESS...";
            _progressBarFill.fillAmount = 0;
            _progressBarCompleted.SetActive(false);
        }

        private void CompleteProgressBar()
        {
            _progressBarText.text = "00:00";
            _progressBarFill.fillAmount = 1;
            _progressBarCompleted.SetActive(true);
        }

        #endregion
    }
}