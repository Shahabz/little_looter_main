/*
 * Date: October 22th, 2023
 * Author: Peche
 */

using DG.Tweening;
using LittleLooters.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_ObjectToRepairPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private PlayerEntryPoint _playerEntry = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtDisplayName = default;
		[SerializeField] private GameObject _content = default;
		[SerializeField] private Slider _progressBar = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtRepairProgress = default;
		[SerializeField] private UI_ObjectToRepairPanel_Slot[] _slots = default;
		[SerializeField] private GameObject _completed = default;

		[Header("Repair button")]
		[SerializeField] private Button _btnRepair = default;
		[SerializeField] private Image _btnRepairBackground = default;
		[SerializeField] private Color _btnRepairColorEnabled = default;
		[SerializeField] private Transform _btnRepairAlert = default;
		[SerializeField] private float _alertAnimationScale = default;
		[SerializeField] private float _alertAnimationDuration = default;

		#endregion

		#region Private properties

		private bool _wasCompleted = false;
		private bool _isReparing = false;
		private int _duration = 0;
		private float _expiration = 0;
		private Sequence _tweenSequence = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			_tweenSequence = DOTween.Sequence()
								.Append(_btnRepairAlert.DOScale(Vector3.one * _alertAnimationScale, _alertAnimationDuration).SetDelay(0.1f))
								.Append(_btnRepairAlert.DOScale(Vector2.one, _alertAnimationDuration))
								.SetLoops(-1, LoopType.Restart);

			PlayerProgressEvents.OnSlotFixDone += HandleSlotFixDone;
		}

		private void OnDestroy()
		{
			PlayerProgressEvents.OnSlotFixDone -= HandleSlotFixDone;
		}

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
			_txtDisplayName.text = data.DisplayName.ToUpperInvariant();

            HideSlots();

			HideProgressBar();

			_completed.SetActive(false);

			for (int i = 0; i < data.Parts.Length; i++)
			{
				if (i >= _slots.Length) break;

				var partData = data.Parts[i];

				var slot = _slots[i];

				slot.Setup(data.Id, partData.resourceData.Id, partData.resourceData.Icon, partData.amount, partData.resourceData.DisplayName);

				slot.gameObject.SetActive(true);
			}
		}

		public void Show()
		{
			_content.SetActive(true);

			RefreshSlots();
		}

		public void Hide()
		{
			_content.SetActive(false);
		}

		public void Refresh(Model.PlayerProgress_ObjectToRepairData data)
		{
			/*
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
			*/
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

		private void RefreshSlots()
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				var slot = _slots[i];

				if (!slot.gameObject.activeSelf) continue;

				var playerResourceAmount = _playerEntry.ProgressData.GetResourceAmount(slot.ResourceId);
				var (_, objectData) = _playerEntry.ProgressData.GetRepairObjectProgressData(slot.ObjectId);
				var objectPartProgressData = objectData.GetPartProgress(slot.ResourceId);

				slot.Refresh(playerResourceAmount, objectPartProgressData.amount, objectPartProgressData.total);
			}
		}

		private void HandleSlotFixDone(PlayerProgressEvents.RepairSlotArgs args)
		{
			if (!args.allSlotsDone) return;

			_btnRepair.interactable = true;

			_btnRepairBackground.color = _btnRepairColorEnabled;

			_btnRepairAlert.gameObject.SetActive(true);

			_tweenSequence.Restart();
		}

		#endregion
	}
}