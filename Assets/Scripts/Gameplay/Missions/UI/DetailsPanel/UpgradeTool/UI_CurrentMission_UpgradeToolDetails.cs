/*
 * Date: November 18th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_CurrentMission_UpgradeToolDetails : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private PlayerEntryPoint _playerEntryPoint = default;
        [SerializeField] private UI_CurrentMission_SlotUpgradeTool[] _slots = default;
        [SerializeField] private GameObject _progressBar = default;
        [SerializeField] private Image _progressBarFill = default;
        [SerializeField] private TMPro.TextMeshProUGUI _progressBarText = default;
        [SerializeField] private GameObject _progressBarCompleted = default;

		#endregion

		#region Private properties

		private bool _inProgress = false;
        private float _duration = 0;
        private float _expiration = 0;

        #endregion

        #region Unity events

        private void OnEnable()
		{
            PlayerProgressEvents.OnResourceHasChanged += ResourceHasChanged;

            PlayerProgressEvents.OnMeleeUpgradeStarted += ToolUpgradeStarted;
            PlayerProgressEvents.OnMeleeUpgradeCompleted += ToolUpgradeCompleted;
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

		public void Setup()
        {
            HideProgressBar();

            HideSlots();

            RefreshSlots();
        }

        private void UnsubscribeEvents()
		{
            PlayerProgressEvents.OnResourceHasChanged -= ResourceHasChanged;

            PlayerProgressEvents.OnMeleeUpgradeStarted -= ToolUpgradeStarted;
            PlayerProgressEvents.OnMeleeUpgradeCompleted -= ToolUpgradeCompleted;
        }

		private void HideSlots()
		{
			for (int i = 0; i < _slots.Length; i++)
			{
                var slot = _slots[i];

                slot.gameObject.SetActive(false);
			}
		}

        private void RefreshSlots()
		{
            var nextLevelData = _playerEntryPoint.GetMeleeNextLevelData();

            var slotsAmount = nextLevelData.requirements.Length;

			for (int i = 0; i < slotsAmount; i++)
			{
                var slot = _slots[i];

                var requirement = nextLevelData.requirements[i];
                var icon = requirement.resource.Icon;
                var currentAmount = _playerEntryPoint.ProgressData.GetResourceAmount(requirement.resource.Id);

                var completed = currentAmount >= requirement.amount;

                slot.Setup(icon, completed);
                slot.gameObject.SetActive(true);
			}
		}

        private void ResourceHasChanged(int id, int amount)
        {
            var nextLevelData = _playerEntryPoint.GetMeleeNextLevelData();

            var slotsAmount = nextLevelData.requirements.Length;

            for (int i = 0; i < slotsAmount; i++)
            {
                var requirement = nextLevelData.requirements[i];
                var resourceId = requirement.resource.Id;

                if (resourceId != id) continue;

                var completed = amount >= requirement.amount;

                if (!completed) return;

                var slot = _slots[i];

                slot.Completed();

                return;
            }
        }

        private void HideProgressBar()
		{
            _progressBar.SetActive(false);
		}

        private void ToolUpgradeCompleted()
        {
            _inProgress = false;

            _progressBarText.text = "CLAIM!";

            _progressBarFill.fillAmount = 1;

            _progressBarCompleted.SetActive(true);
        }

        private void ToolUpgradeStarted(PlayerProgressEvents.MeleeUpgradeStartedArgs args)
        {
            _duration = args.duration;
            _expiration = args.expiration;

            HideSlots();

            _progressBarCompleted.SetActive(false);

            _progressBar.SetActive(true);

            RefreshProgress();

            _inProgress = true;
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
    }
}