/*
 * Date: December 16th, 2023
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_Crafting_InProgressPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _content = default;
        [SerializeField] private Button _btnSkip = default;
        [SerializeField] private Image _icon = default;
        [SerializeField] private Slider _progressBar = default;
        [SerializeField] private TMPro.TextMeshProUGUI _txtTime = default;

        #endregion

        #region Private properties

        private int _areaId = default;
		private bool _inProgress = false;
        private float _expiration = 0;
        private int _duration = 0;
        private float _remainingTime = 0;

		#endregion

		#region Unity events

		private void Update()
		{
            if (!_inProgress) return;

            RefreshProgressBar();
		}

		private void OnEnable()
		{
            UI_GameplayEvents.OnCloseSkipTimePanel += HandleOnCloseSkipTimePanel;

            _btnSkip.onClick.AddListener(ProcessSkip);
		}

        private void OnDisable()
		{
            UI_GameplayEvents.OnCloseSkipTimePanel -= HandleOnCloseSkipTimePanel;

            _btnSkip.onClick.RemoveAllListeners();
        }

		#endregion

		#region Public methods

		public void Show(int areaId, Sprite icon, float expiration, int duration)
		{
            _areaId = areaId;

            _icon.sprite = icon;

            _expiration = expiration;

            var now = Time.time;
            _remainingTime = _expiration - now;

            _duration = duration;

            InvokeRepeating(nameof(RefreshTime), 0, 1);

            RefreshProgressBar();

            _inProgress = _remainingTime > 0;

            _content.SetActive(true);
		}

        public void Hide()
		{
            _inProgress = false;

            _content.SetActive(false);

            CancelInvoke(nameof(RefreshTime));
        }

        public void RefreshExpiration(float expiration)
		{
            _expiration = expiration;

            var now = Time.time;
            _remainingTime = _expiration - now;

            CancelInvoke(nameof(RefreshTime));

            InvokeRepeating(nameof(RefreshTime), 0, 1);

            RefreshProgressBar();

            _inProgress = _remainingTime > 0;
        }

		#endregion

		#region Private methods

		private void RefreshTime()
        {
            var now = Time.time;
            var remainingTime = Mathf.CeilToInt(_expiration - now);

            if (remainingTime < 0)
			{
                remainingTime = 0;
			}

            _txtTime.text = UI_Utils.GetFormatTime(remainingTime);
        }

        private void RefreshProgressBar()
        {
            _remainingTime -= Time.deltaTime;

            var progress = 1 - (_remainingTime / _duration);

            _progressBar.value = progress;

            _inProgress = _remainingTime > 0;
        }

        private void ProcessSkip()
		{
            Hide();

            UI_GameplayEvents.OnCraftingSkipped?.Invoke(_areaId);
		}

        private void HandleOnCloseSkipTimePanel(UI_ToolUpgradeSkipPanel.SkipPanelOpeningReason reason)
        {
            if (reason != UI_ToolUpgradeSkipPanel.SkipPanelOpeningReason.CRAFTING) return;

            InvokeRepeating(nameof(RefreshTime), 0, 1);

            RefreshProgressBar();

            _inProgress = true;

            _content.SetActive(true);
        }

        #endregion
    }
}