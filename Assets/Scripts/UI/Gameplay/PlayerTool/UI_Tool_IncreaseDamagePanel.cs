/*
 * Date: November 29th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_Tool_IncreaseDamagePanel : MonoBehaviour
    {
		[SerializeField] private GameObject _content = default;
		[SerializeField] private Button _btn = default;
		[SerializeField] private float _delay = default;
		[SerializeField] private float _hidenDuration = default;
		[SerializeField] private GameObject _regularPanel = default;
		[SerializeField] private GameObject _freePanel = default;
		[SerializeField] private GameObject _adPanel = default;

		[Header("Progress")]
		[SerializeField] private GameObject _inProgressPanel = default;
		[SerializeField] private GameObject _progressPanel = default;
		[SerializeField] private TextMeshProUGUI _txtTime = default;
		[SerializeField] private GameObject _progressPanelVfx = default;
		[SerializeField] private Image _progressFill = default;

		private bool _inProgress = true;
		private float _expiration = default;
		private float _duration = default;

		private void OnEnable()
		{
			PlayerProgressEvents.OnToolDamageIncreaseStarted += HandleToolExtraDamageStarted;
			PlayerProgressEvents.OnToolDamageIncreaseCompleted += HandleToolExtraDamageCompleted;

			_btn.onClick.AddListener(Apply);
		}

		private void OnDisable()
		{
			PlayerProgressEvents.OnToolDamageIncreaseStarted -= HandleToolExtraDamageStarted;
			PlayerProgressEvents.OnToolDamageIncreaseCompleted -= HandleToolExtraDamageCompleted;

			_btn.onClick.RemoveAllListeners();
		}

		private void Update()
		{
			if (!_inProgress) return;

			RefreshProgress();
		}

		private void Apply()
		{
			// TODO: SFX

			_btn.interactable = false;

			UI_GameplayEvents.OnIncreaseToolDamage?.Invoke();
		}

		private void HandleToolExtraDamageStarted(PlayerProgressEvents.ToolExtraDamageStartedArgs args)
		{
			_inProgress = true;

			_expiration = args.expiration;

			_duration = args.duration;

			// Show progress panel
			_progressPanel.SetActive(true);
			_progressPanelVfx.SetActive(true);

			_regularPanel.SetActive(false);
			_inProgressPanel.SetActive(true);
		}

		private void HandleToolExtraDamageCompleted()
		{
			_inProgress = false;

			_btn.interactable = true;

			// Hide progress panel
			_progressPanel.SetActive(false);
			_progressPanelVfx.SetActive(false);

			_regularPanel.SetActive(true);
			_inProgressPanel.SetActive(false);
		}

		private void RefreshProgress()
		{
			var now = Time.time;
			var remainingTime = Mathf.FloorToInt(_expiration - now);

			var timeText = UI_Utils.GetFormatTime(remainingTime);

			_txtTime.text = timeText;

			var progress = (_expiration - now) / _duration;

			_progressFill.fillAmount = progress;
		}
	}
}