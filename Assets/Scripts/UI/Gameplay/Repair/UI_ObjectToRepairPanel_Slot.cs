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
		#region Inspector

		[SerializeField] private TMPro.TextMeshProUGUI _txtDisplayName = default;
        [SerializeField] private TMPro.TextMeshProUGUI _txtAmount = default;
        [SerializeField] private Image _icon = default;
        [SerializeField] private Image _progressBar = default;
        [SerializeField] private Image _completed = default;
        [SerializeField] private Button _btnFix = default;
        [SerializeField] private Image _btnFixBackground = default;
        [SerializeField] private Color _colorFixEnabled = default;
        [SerializeField] private Color _colorFixDisabled = default;

		#endregion

		#region Private properties

		private int _objectId = default;
        private int _resourceId = default;
        private bool _isEnabled = false;
        private bool _isCompleted = false;

		#endregion

		#region Public properties

		public int ResourceId => _resourceId;
        public int ObjectId => _objectId;

		#endregion

		#region Unity events

		private void OnEnable()
		{
            _btnFix.onClick.AddListener(Fix);

            PlayerProgressEvents.OnSlotFixDone += HandleSlotFixDone;
        }

        private void OnDisable()
        {
            _btnFix.onClick.RemoveAllListeners();

            PlayerProgressEvents.OnSlotFixDone -= HandleSlotFixDone;
        }

		#endregion

		#region Public methods

		public void Setup(int objectId, int slotId, Sprite icon, int total, string displayName)
		{
            _objectId = objectId;
            _resourceId = slotId;

            _txtDisplayName.text = displayName;
            _txtAmount.text = $"0/{total}";

            _icon.sprite = icon;

            RefreshProgressBar(0);

            RefreshCompleted(false);
		}

        public void Refresh(int playerResourceAmount, int currentProgress, int totalProgress)
		{
            if (_isCompleted) return;

            if (playerResourceAmount == 0)
			{
                MarkAsDisable();
                return;
			}

            MarkAsEnable(currentProgress, totalProgress);
		}

		/*public void Refresh(PartProgress partProgress)
		{
            
            var amount = partProgress.amount;
            var total = partProgress.total;

            var remaining = total - amount;

            _txtAmount.text = $"x{remaining}";

            var progress = (float)partProgress.amount / (float)partProgress.total;

            RefreshProgressBar(progress);

            RefreshCompleted(progress == 1);
        }*/

		#endregion

		#region Private methods

		private void RefreshProgressBar(float progress)
		{
            _progressBar.fillAmount = progress;
		}

        private void RefreshCompleted(bool isCompleted)
		{
            _completed.enabled = isCompleted;

            _txtAmount.enabled = !isCompleted;

        }

        private void MarkAsComplete()
		{
            _isEnabled = false;

            _btnFix.gameObject.SetActive(false);

            _completed.enabled = true;
		}

        private void MarkAsDisable()
		{
            _isEnabled = false;

            _btnFixBackground.color = _colorFixDisabled;
		}

        private void MarkAsEnable(int currentProgress, int totalProgress)
        {
            _isEnabled = true;

            _btnFixBackground.color = _colorFixEnabled;

            _txtAmount.text = $"{currentProgress}/{totalProgress}";
        }

        private void Fix()
		{
            // TODO: sfx

            if (_canDebug) DebugFix();

            if (!_isEnabled) return;

            UI_GameplayEvents.OnFixSlot?.Invoke(_objectId, _resourceId);
		}

        private void HandleSlotFixDone(PlayerProgressEvents.RepairSlotArgs args)
        {
            if (args.objectId != _objectId) return;

            if (args.resourceId != _resourceId) return;

            _txtAmount.text = $"{args.currentAmount}/{args.totalAmount}";

            _isCompleted = args.currentAmount == args.totalAmount;

            if (_isCompleted)
			{
                MarkAsComplete();
                return;
			}

            MarkAsDisable();
        }

        #endregion

        #region Debug

        private bool _canDebug = false;

        private void DebugFix()
		{
            Debug.LogError($"FIX -> <color=yellow>'{_objectId}'</color>, slot <color=cyan>'{_resourceId}'</color>, is enabled: <color=orange>'{_isEnabled}'</color>");
		}

		#endregion
	}
}