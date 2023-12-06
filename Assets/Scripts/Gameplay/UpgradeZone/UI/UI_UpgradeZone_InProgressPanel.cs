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
		[SerializeField] private Slider _progressBar = default;
		[SerializeField] private Button _btnSkip = default;
		[SerializeField] private TextMeshProUGUI _txtCurrentDamage = default;
		[SerializeField] private TextMeshProUGUI _txtNextDamage = default;

		#endregion

		#region Private properties

		private bool _inProgress = false;
		private float _duration = 0;
		private float _expiration = 0;
		private float _remainingTime = 0;

		#endregion

		#region Public methods

		public void Show()
		{
			_panel.SetActive(true);
			_canvas.enabled = true;

			RefreshProgressTime();
			RefreshProgressBar();
		}

		public void Hide()
		{
			_panel.SetActive(false);
			_canvas.enabled = false;
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

			RefreshProgressTime();

			RefreshProgressBar();
		}

		#endregion

		#region Private methods

		private void RefreshProgressTime()
		{
			var now = Time.time;
			var remainingTime = Mathf.CeilToInt(_expiration - now);

			_txtTime.text = UI_Utils.GetFormatTime(remainingTime);
		}

		private void RefreshProgressBar()
		{
			_remainingTime -= Time.deltaTime;

			var progress = _remainingTime / _duration;

			_progressBar.value = progress;
		}

		private void UpgradeStarted(PlayerProgressEvents.MeleeUpgradeStartedArgs args)
		{
			_inProgress = true;

			_duration = args.duration;
			_expiration = args.expiration;
			_remainingTime = args.duration;

			RefreshDamage(args.currentDamage, args.nextLevelDamage);

			Show();
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

		private void RefreshDamage(int current, int next)
		{
			var extraDamage = next - current;

			_txtCurrentDamage.text = $"{current}";
			_txtNextDamage.text = $"+{extraDamage}";
		}

		#endregion
	}
}