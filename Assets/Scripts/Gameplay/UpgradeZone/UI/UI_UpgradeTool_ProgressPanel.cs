/*
 * Date: February 29th, 2024
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_UpgradeTool_ProgressPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _content = default;
        [SerializeField] private TextMeshProUGUI _txtTime = default;
        [SerializeField] private Slider _progressBar = default;

        #endregion

        #region Private properties

        private bool _inProgress = false;
        private float _duration = 0;
        private float _expiration = 0;
        private float _remainingTime = 0;

        #endregion

        #region Unity events

        private void Update()
        {
            if (!_inProgress) return;

            RefreshProgressTime();

            RefreshProgressBar();
        }

		#endregion

		#region Public methods

		public void Show()
        {
            LoadInformation();

            _content.SetActive(true);
        }

        public void Hide()
        {
            _content.SetActive(false);
        }

		#endregion

		#region Private methods

		private void LoadInformation()
		{
            _inProgress = true;

            var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();

            var toolData = progressDataService.ProgressData.toolData;

            _duration = toolData.upgradeDuration;
            _expiration = toolData.upgradeExpiration;
            _remainingTime = _expiration - Time.time;

            RefreshProgressTime();
            RefreshProgressBar();
        }

        private void RefreshProgressTime()
        {
            var now = Time.time;
            var remainingTime = Mathf.CeilToInt(_expiration - now);

            _txtTime.text = UI_Utils.GetFormatTime(remainingTime);
        }

        private void RefreshProgressBar()
        {
            _remainingTime -= Time.deltaTime;

            var progress = 1 - (_remainingTime / _duration);

            _progressBar.value = progress;
        }

		#endregion
	}
}