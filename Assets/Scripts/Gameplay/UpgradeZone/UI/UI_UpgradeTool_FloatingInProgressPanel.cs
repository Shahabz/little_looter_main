/*
 * Date: March 23th, 2024
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_UpgradeTool_FloatingInProgressPanel : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private GameObject _content = default;
        [SerializeField] private TextMeshProUGUI _txtTime = default;
        [SerializeField] private Slider _progressBar = default;
        [SerializeField] private Button _btnSkip = default;

        #endregion

        #region Private properties

        private bool _inProgress = false;
        private float _duration = 0;
        private float _expiration = 0;
        private float _remainingTime = 0;

        #endregion

        #region Unity events

        private void Awake()
        {
            UI_GameplayEvents.OnStartGame += HandleGameStarted;
            UI_GameplayEvents.OnShowUpgradeToolProgress += HandleShowProgressPanel;
            PlayerProgressEvents.OnMeleeUpgradeStarted += HandleToolUpgradeStarted;
            PlayerProgressEvents.OnToolUpgradeExpirationHasChanged += HandleToolUpgradeExpirationChanged;
            PlayerProgressEvents.OnMeleeUpgradeCompleted += HandleToolUpgradeCompleted;

            _btnSkip.onClick.AddListener(Skip);
        }

		private void OnDestroy()
        {
            UI_GameplayEvents.OnStartGame -= HandleGameStarted;
            UI_GameplayEvents.OnShowUpgradeToolProgress -= HandleShowProgressPanel;
            PlayerProgressEvents.OnMeleeUpgradeStarted -= HandleToolUpgradeStarted;
            PlayerProgressEvents.OnToolUpgradeExpirationHasChanged -= HandleToolUpgradeExpirationChanged;
            PlayerProgressEvents.OnMeleeUpgradeCompleted -= HandleToolUpgradeCompleted;

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

        private void Show()
        {
            LoadInformation();

            _content.SetActive(true);
        }

        private void Hide()
        {
            _content.SetActive(false);
        }

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

        private void Skip()
        {
            // TODO: play SFX

            // Close
            Hide();

            UI_GameplayEvents.OnSkipToolUpgrade?.Invoke();
        }

        #endregion

        #region Events

        private void HandleGameStarted()
		{
            Hide();
		}

        private void HandleShowProgressPanel()
        {
            Show();
        }

        private void HandleToolUpgradeStarted(PlayerProgressEvents.MeleeUpgradeStartedArgs args)
        {
            Show();
        }

        private void HandleToolUpgradeExpirationChanged(PlayerProgressEvents.ToolUpgradeExpirationChangedArgs args)
        {
            Show();
        }

        private void HandleToolUpgradeCompleted()
        {
            Hide();
        }

        #endregion
    }
}