/*
 * Date: October 1st, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay
{
	/// <summary>
	/// Represents the behavior of repairable objects.
	/// When player interacts with it, based on its status, it can be repaired based on elements needed and repairing speed.
	/// </summary>
    public class RepairObject : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _indicator = default;
		[SerializeField] private Slider _progressBar = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtRepairing = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtRepairProgress = default;
		[SerializeField] private Image _completed = default;
		[SerializeField] private RepairObjectData _data = default;
		[SerializeField] private GameObject _neededPanel = default;
		[SerializeField] private Image _neededPartIcon = default;
		[SerializeField] private float _delayToHideNeeded = 3f;
		[SerializeField] private UI_ObjectToRepairPanel _uiPanel = default;

		#endregion

		#region Private properties

		private float _progress = 0;
		private float _progressGoal = 10;
		private float _speedRepairing = 0;
		private const float _step = 0.1f;
		private bool _isProcessing = false;
		private bool _wasCompleted = false;
		private System.Action _onComplete = default;

		#endregion

		#region Public properties

		public int Id => _data.Id;
		public bool IsRepairing => _isProcessing;
		public bool WasCompleted => _wasCompleted;

		public RepairObjectData Data => _data;

		#endregion

		#region Unity events

		private void Start()
		{
			_uiPanel.Setup(_data);
			_uiPanel.Hide();
		}

		#endregion

		#region Public methods

		public void ShowIndicator()
		{
			//if (_wasCompleted) return;

			_indicator.SetActive(true);

			_uiPanel.Show();
		}

		public void HideIndicator()
		{
            _indicator.SetActive(false);

			_uiPanel.Hide();
        }

        public void StartProcess(RepairPartData data, float speedRepairing, System.Action onComplete)
		{
			if (_wasCompleted) return;

			_onComplete = onComplete;

			Debug.LogError("<color=green>START</color>");

			_isProcessing = true;

			_txtRepairing.enabled = true;

			_speedRepairing = speedRepairing;

			InvokeRepeating(nameof(Process), 0, _step);
		}

        public void StopProcess()
		{
			_isProcessing = false;

			_txtRepairing.enabled = false;

			CancelInvoke(nameof(Process));

			Debug.LogError("<color=red>STOP</color>");
		}

		public void ShowNeeded(RepairPartData data)
		{
			_neededPartIcon.sprite = data.Icon;
			_neededPanel.SetActive(true);

			Invoke(nameof(HideNeeded), _delayToHideNeeded);
		}

		public void RefreshState(Model.PlayerProgress_ObjectToRepairData data)
		{
			_uiPanel.Refresh(data);
		}

		#endregion

		#region Private methods

		private void Process()
		{
			/*
			_progress += _speedRepairing * _step;

			var progress = Mathf.Clamp(_progress / _progressGoal, 0, 1);

			_progressBar.value = progress;

			var clampedProgress = Mathf.FloorToInt(progress * 100);

			_txtRepairProgress.text = $"{clampedProgress}%";

			Debug.LogError($"Process: <color=magenta>{progress}</color>");

			if (progress < 1) return;

			CompleteProcess();
			*/
		}

		private void CompleteProcess()
		{
			/*
			HideIndicator();

			_completed.enabled = true;

			_wasCompleted = true;

			_isProcessing = false;

			_txtRepairing.enabled = false;

			CancelInvoke(nameof(Process));

			Debug.LogError("<color=orange>COMPLETED!</color>");

			_onComplete?.Invoke();

			_onComplete = null;

			ConsumePart();

			_currentPartInUse = -1;
			*/
		}

		private void HideNeeded()
		{
			_neededPanel.SetActive(false);
		}

		#endregion
	}
}