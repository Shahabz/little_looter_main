/*
 * Date: October 22th, 2023
 * Author: Peche
 */

using DG.Tweening;
using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_ObjectToRepairPanel : MonoBehaviour
    {
		#region Inspector

		//[SerializeField] private GameObject _content = default;
		[SerializeField] private VisibleObject _visibleTarget = default;
		[SerializeField] private GameObject _ui = default;

		[Header("Slots panel")]
		[SerializeField] private GameObject _slotsPanel = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtDisplayName = default;
		[SerializeField] private UI_ObjectToRepairPanel_Slot[] _slots = default;
		
		[Header("Repair button")]
		[SerializeField] private Button _btnRepair = default;
		[SerializeField] private Image _btnRepairBackground = default;
		[SerializeField] private Color _btnRepairColorEnabled = default;
		[SerializeField] private Transform _btnRepairAlert = default;
		[SerializeField] private float _alertAnimationScale = default;
		[SerializeField] private float _alertAnimationDuration = default;

		[Header("Progress panel")]
		[SerializeField] private GameObject _progressPanel = default;
		[SerializeField] private Slider _progressBar = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtRepairProgress = default;
		[SerializeField] private Button _btnSkip = default;

		[Header("Completed panel")]
		[SerializeField] private GameObject _completedPanel = default;

		#endregion

		#region Private properties

		private int _id = default;
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

			UI_GameplayEvents.OnShowRepairPanel += HandleShowPanel;
			UI_GameplayEvents.OnHideRepairPanel += HandleHidePanel;

			PlayerProgressEvents.OnSlotFixDone += HandleSlotFixDone;
			PlayerProgressEvents.OnStartRepairing += HandleRepairingStarted;
			PlayerProgressEvents.OnCompleteRepairing += HandleRepairingCompleted;
			PlayerProgressEvents.OnSpeedUpRepairing += HandleRepairingSpeedUp;
		}

		private void Start()
		{
			_visibleTarget.OnStatusChanged += HandleVisibleObjectStatusChanged;

			//Hide();
		}

		private void OnEnable()
		{
			_btnRepair.onClick.AddListener(StartRepairingInput);
			_btnSkip.onClick.AddListener(Skip);
		}

		private void OnDisable()
		{
			_btnRepair.onClick.RemoveAllListeners();
			_btnSkip.onClick.RemoveAllListeners();
		}

		private void OnDestroy()
		{
			_visibleTarget.OnStatusChanged -= HandleVisibleObjectStatusChanged;

			UI_GameplayEvents.OnShowRepairPanel -= HandleShowPanel;
			UI_GameplayEvents.OnHideRepairPanel -= HandleHidePanel;

			PlayerProgressEvents.OnSlotFixDone -= HandleSlotFixDone;
			PlayerProgressEvents.OnStartRepairing -= HandleRepairingStarted;
			PlayerProgressEvents.OnCompleteRepairing -= HandleRepairingCompleted;
			PlayerProgressEvents.OnSpeedUpRepairing -= HandleRepairingSpeedUp;
		}

		private void Update()
		{
			if (_wasCompleted) return;

			if (!_isReparing) return;

			ShowRepairProgress(_duration, _expiration);
		}

		#endregion

		#region Private methods

		private void HandleShowPanel(RepairObjectData data)
		{
			Setup(data);
			Show();
		}

		private void HandleHidePanel()
		{
			//Hide();
		}

		private void Setup(RepairObjectData data)
		{
			_id = data.Id;

			_txtDisplayName.text = data.DisplayName.ToUpperInvariant();

            HideSlots();

			HideProgressBar();

			HideCompleted();

			for (int i = 0; i < data.Parts.Length; i++)
			{
				if (i >= _slots.Length) break;

				var partData = data.Parts[i];

				var slot = _slots[i];

				slot.Setup(data.Id, partData.resourceData.Id, partData.resourceData.Icon, partData.amount, partData.resourceData.DisplayName);

				slot.gameObject.SetActive(true);
			}
		}

		private void Show()
		{
			//_content.SetActive(true);

			if (_wasCompleted)
			{
				ShowCompleted();
				HideSlotsPanel();
				HideProgressBar();
				return;
			}

			if (_isReparing)
			{
				ShowProgressBar();
				HideCompleted();
				HideSlotsPanel();
				return;
			}

			ShowSlotsPanel();
			RefreshSlots();
			HideProgressBar();
			HideCompleted();
		}

		private void Hide()
		{
			//_content.SetActive(false);

			HideCompleted();
			HideSlotsPanel();
			HideSlotsPanel();
		}

		private void HideSlotsPanel()
		{
			_slotsPanel.SetActive(false);
		}

		private void ShowSlotsPanel()
		{
			// NOTE: uncomment this line
			//_slotsPanel.SetActive(true);
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
			_progressPanel.SetActive(false);
		}

		private void ShowProgressBar()
		{
			_progressPanel.SetActive(true);
		}

		private void ShowRepairProgress(int duration, float expiration)
		{
			var now = Time.time;
			var remainingTime = expiration - now;

			var secs = remainingTime;
			var mins = Mathf.FloorToInt(secs / 60);

			secs = Mathf.FloorToInt(secs - mins * 60);

			_txtRepairProgress.text = $"{mins:00}:{secs:00}";	// TODO: use UTILS functionality to get formated time

			var progress = 1 - remainingTime / duration;

			_progressBar.value = progress;
		}

		private void RefreshSlots()
		{
			var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();


			for (int i = 0; i < _slots.Length; i++)
			{
				var slot = _slots[i];

				if (!slot.gameObject.activeSelf) continue;

				var playerResourceAmount = progressDataService.ProgressData.GetResourceAmount(slot.ResourceId);
				var (_, objectData) = progressDataService.ProgressData.GetRepairObjectProgressData(slot.ObjectId);
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

		private void StartRepairingInput()
		{
			// TODO: SFX

			HideSlots();

			UI_GameplayEvents.OnStartRepairing?.Invoke(_id);
		}

		private void HandleRepairingCompleted(int id)
		{
			if (_id != id) return;

			_isReparing = false;
			_wasCompleted = true;
			_expiration = 0;

			HideSlots();
			HideProgressBar();
			ShowCompleted();
		}

		private void HandleRepairingStarted(PlayerProgressEvents.RepairStartActionArgs args)
		{
			if (_id != args.id) return;

			_expiration = args.expiration;
			_duration = args.duration;
			_isReparing = true;
			_wasCompleted = false;

			HideSlotsPanel();
			ShowProgressBar();
			HideCompleted();
		}

		private void HandleRepairingSpeedUp(PlayerProgressEvents.RepairSpeedUpArgs args)
		{
			if (!_isReparing) return;

			if (_id != args.id) return;

			_expiration = args.expiration;
			_duration = args.duration;
		}

		private void ShowCompleted()
		{
			_completedPanel.SetActive(true);
		}

		private void HideCompleted()
		{
			_completedPanel.SetActive(false);
		}

		private void Skip()
		{
			// TODO: SFX

			UI_GameplayEvents.OnSkipRepairing?.Invoke(_id, _expiration, _duration);
		}

		private void HandleVisibleObjectStatusChanged(bool isVisible)
		{
			_ui.SetActive(isVisible);
		}

		#endregion
	}
}