/*
 * Date: October 22th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_ObjectToRepairPanel_Slot : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _txtAmount = default;
        [SerializeField] private Image _icon = default;
        [SerializeField] private Image _progressBar = default;
        [SerializeField] private Image _completed = default;

        private int _id = default;

        public int Id => _id;

        public void Setup(PartsData partData)
		{
            _id = partData.data.Id;

            var amount = partData.amount;
            var icon = partData.data.Icon;

            _txtAmount.text = $"x{amount}";

            _icon.sprite = icon;

            RefreshProgressBar(0);

            RefreshCompleted(false);
		}

        public void Refresh(PartProgress partProgress)
		{
            var amount = partProgress.amount;
            var total = partProgress.total;

            var remaining = total - amount;

            _txtAmount.text = $"x{remaining}";

            var progress = (float)partProgress.amount / (float)partProgress.total;

            RefreshProgressBar(progress);

            RefreshCompleted(progress == 1);
        }

        private void RefreshProgressBar(float progress)
		{
            _progressBar.fillAmount = progress;
		}

        private void RefreshCompleted(bool isCompleted)
		{
            _completed.enabled = isCompleted;

            _txtAmount.enabled = !isCompleted;

        }
    }
}