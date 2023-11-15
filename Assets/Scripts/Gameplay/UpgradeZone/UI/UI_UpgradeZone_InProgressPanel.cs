/*
 * Date: November 6th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_UpgradeZone_InProgressPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _panel = default;
		[SerializeField] private Canvas _canvas = default;
		[SerializeField] private TextMeshProUGUI _txtTime = default;
		[SerializeField] private Image _progressBar = default;
		[SerializeField] private Button _btnSkip = default;

		#endregion

		#region Private properties

		private bool _inProgress = false;
		private float _duration = 0;
		private float _expiration = 0;

		#endregion

		#region Public methods

		public void Show(float duration, float expiration)
		{
			_panel.SetActive(true);
			_canvas.enabled = true;

			_inProgress = true;

			_duration = duration;
			_expiration = expiration;

			RefreshProgress();
		}

		public void Hide()
		{
			_panel.SetActive(false);
			_canvas.enabled = false;

			_inProgress = false;
		}

		#endregion

		#region Unity events

		private void Awake()
		{
			PlayerProgressEvents.OnMeleeUpgradeStarted += UpgradeStarted;
			PlayerProgressEvents.OnMeleeUpgradeCompleted += UpgradeCompleted;

			_btnSkip.onClick.AddListener(Skip);
		}

		private void OnDestroy()
		{
			PlayerProgressEvents.OnMeleeUpgradeStarted -= UpgradeStarted;
			PlayerProgressEvents.OnMeleeUpgradeCompleted -= UpgradeCompleted;

			_btnSkip.onClick.RemoveAllListeners();
		}

		private void Update()
		{
			if (!_inProgress) return;

			RefreshProgress();
		}

		#endregion

		#region Private methods

		private void RefreshProgress()
		{
			var now = Time.time;
			var remainingTime = _expiration - now;

			var secs = remainingTime;
			var mins = Mathf.FloorToInt(secs / 60);

			secs = Mathf.CeilToInt(secs - mins * 60);

			_txtTime.text = $"{mins:00}:{secs:00}";

			var progress = 1 - remainingTime / _duration;

			_progressBar.fillAmount = progress;
		}

		private void UpgradeStarted(PlayerProgressEvents.MeleeUpgradeStartedArgs args)
		{
			Show(args.duration, args.expiration);
		}

		private void UpgradeCompleted()
		{
			_inProgress = false;

			Hide();
		}

		private void Skip()
		{
			// TODO: play SFX

			UI_GameplayEvents.OnSkipToolUpgrade?.Invoke();
		}

		#endregion
	}
}