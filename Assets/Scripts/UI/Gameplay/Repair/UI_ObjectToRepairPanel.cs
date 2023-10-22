/*
 * Date: October 22th, 2023
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_ObjectToRepairPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _content = default;
		[SerializeField] private Slider _progressBar = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtRepairProgress = default;
		[SerializeField] private UI_ObjectToRepairPanel_Slot[] _slots = default;
		[SerializeField] private GameObject _completed = default;

		#endregion

		#region Private properties

		private bool _wasCompleted = false;
		private bool _isReparing = false;
		private int _duration = 0;
		private float _expiration = 0;

		#endregion

		#region Unity events

		private void Update()
		{
			if (_wasCompleted) return;

			if (!_isReparing) return;

			ShowRepairProgress(_duration, _expiration);

			CheckCompletion();
		}

		#endregion

		#region Public methods

		public void Setup(RepairObjectData data)
		{
            HideSlots();

			HideProgressBar();

			_completed.SetActive(false);

			for (int i = 0; i < data.Parts.Length; i++)
			{
				if (i >= _slots.Length) break;

				var partData = data.Parts[i];

				var slot = _slots[i];

				slot.Setup(partData);

				slot.gameObject.SetActive(true);
			}
		}

		public void Show()
		{
			_content.SetActive(true);
		}

		public void Hide()
		{
			_content.SetActive(false);
		}

		public void Refresh(Model.PlayerProgress_ObjectToRepairData data)
		{
			if (_wasCompleted) return;

			for (int i = 0; i < _slots.Length; i++)
			{
				var slot = _slots[i];

				if (!slot.gameObject.activeSelf) continue;

				var slotId = slot.Id;

				var partProgressData = data.GetPartProgress(slotId);

				slot.Refresh(partProgressData);
			}

			var isRepairing = data.AllPartsCompleted();

			if (!isRepairing) return;

			var wasRepaired = Time.time >= data.expiration;

			if (wasRepaired)
			{
				MarkCompleted();

				return;
			}

			_isReparing = true;

			ShowProgressBar();

			_duration = data.duration;

			_expiration = data.expiration;

			ShowRepairProgress(data.duration, data.expiration);
		}

		#endregion

		#region Private methods

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
			_progressBar.gameObject.SetActive(false);
		}

		private void ShowProgressBar()
		{
			_progressBar.gameObject.SetActive(true);
		}

		private void ShowRepairProgress(int duration, float expiration)
		{
			var now = Time.time;
			var remainingTime = expiration - now;

			var secs = remainingTime;
			var mins = Mathf.FloorToInt(secs / 60);

			secs = Mathf.FloorToInt(secs - mins * 60);

			_txtRepairProgress.text = $"{mins:00}:{secs:00}";

			var progress = 1 - remainingTime / duration;

			_progressBar.value = progress;
		}

		private void CheckCompletion()
		{
			var now = Time.time;
			var remainingTime = _expiration - now;

			if (remainingTime > 0) return;

			MarkCompleted();
		}

		private void MarkCompleted()
		{
			_completed.SetActive(true);

			_wasCompleted = true;

			HideProgressBar();
		}

		#endregion
	}
}