/*
 * Date: December 29th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_CurrentMission_RepairingDetails : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private PlayerEntryPoint _playerEntryPoint = default;
        [SerializeField] private UI_CurrentMission_RepairingSlot[] _slots = default;
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
            PlayerProgressEvents.OnResourceHasChanged += HandleResourceHasChanged;
            PlayerProgressEvents.OnStartRepairing += HandleProcessStarted;
            PlayerProgressEvents.OnCompleteRepairing += HandleProcessCompleted;
            PlayerProgressEvents.OnSpeedUpRepairing += HandleProcessSpeedUp;
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

        public void Setup(RepairObjectData data)
        {
            _id = data.Id;

            HideProgressBar();

            HideSlots();

            SetupSlots(data);
        }

        #endregion

        #region Private methods

        private void UnsubscribeEvents()
        {
            PlayerProgressEvents.OnResourceHasChanged -= HandleResourceHasChanged;
            PlayerProgressEvents.OnSpeedUpRepairing -= HandleRepairingSpeedUp;
            PlayerProgressEvents.OnStartRepairing -= HandleProcessStarted;
            PlayerProgressEvents.OnCompleteRepairing -= HandleProcessCompleted;
            PlayerProgressEvents.OnSpeedUpRepairing -= HandleProcessSpeedUp;
        }

		private void HideSlots()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                var slot = _slots[i];

                slot.gameObject.SetActive(false);
            }
        }

        private void HideProgressBar()
        {
            _progressBar.SetActive(false);
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

        private void SetupSlots(RepairObjectData data)
		{
            var partsToRepair = data.Parts;

			for (int i = 0; i < _slots.Length; i++)
			{
                if (i >= partsToRepair.Length) break;

                var slot = _slots[i];

                var part = partsToRepair[i];
                var fixedAmount = _playerEntryPoint.ProgressData.GetSlotRepairStatus(_id, part.resourceData.Id);
                var currentAmount = _playerEntryPoint.ProgressData.GetResourceAmount(_id);

                slot.Setup(part, fixedAmount, currentAmount);

                slot.gameObject.SetActive(true);
			}
		}

        private void HandleRepairingSpeedUp(PlayerProgressEvents.RepairSpeedUpArgs args)
		{
            if (!_inProgress) return;

            _expiration = args.expiration;

            RefreshProgress();
        }

        private void HandleResourceHasChanged(int id, int amount)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                var slot = _slots[i];

                if (slot.Id != id) continue;

                var currentAmount = _playerEntryPoint.ProgressData.GetResourceAmount(id);

                slot.Refresh(currentAmount);
            }
        }

        private void HandleProcessStarted(PlayerProgressEvents.RepairStartActionArgs args)
        {
            _inProgress = true;

            _duration = args.duration;

            _expiration = args.expiration;

            HideSlots();

            _progressBarCompleted.SetActive(false);

            _progressBar.SetActive(true);

            RefreshProgress();
        }

        private void HandleProcessCompleted(int id)
        {
            if (_id != id) return;

            _progressBarCompleted.SetActive(true);

            _inProgress = false;
        }


        private void HandleProcessSpeedUp(PlayerProgressEvents.RepairSpeedUpArgs args)
        {
            if (_id != args.id) return;

            _expiration = args.expiration;

            RefreshProgress();
        }

        #endregion
    }
}