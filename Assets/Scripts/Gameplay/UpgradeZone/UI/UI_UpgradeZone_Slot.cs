/*
 * Date: November 5th, 2023
 * Author: Peche
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_UpgradeZone_Slot : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private TextMeshProUGUI _txtRequired = default;
		[SerializeField] private Image _icon = default;
		[SerializeField] private Color _colorCompleted = default;
		[SerializeField] private Color _colorNotCompleted = default;
		[SerializeField] private Image _toggleCompleted = default;
		[SerializeField] private Button _btnInfo = default;
		[SerializeField] private GameObject _progressBar = default;
		[SerializeField] private Image _progressBarFill = default;

		#endregion

		#region Private properties

		private int _resourceId = -1;

		#endregion

		#region Unity events

		private void OnEnable()
		{
			_btnInfo.onClick.AddListener(ShowInfo);
		}

		private void OnDisable()
		{
			_btnInfo.onClick.RemoveAllListeners();
		}

		#endregion

		#region Public methods

		public void Setup(int resourceId, int currentAmount, int requiredAmount, Sprite icon)
		{
			_resourceId = resourceId;

			var completed = currentAmount >= requiredAmount;

			_txtRequired.text = $"x{requiredAmount}";
			_txtRequired.color = (completed) ? _colorCompleted : _colorNotCompleted;

			_progressBar.SetActive(!completed);
			_progressBarFill.fillAmount = (float)currentAmount / (float)requiredAmount;

			_toggleCompleted.enabled = completed;

			_icon.sprite = icon;
		}

		#endregion

		#region Private methods

		private void ShowInfo()
		{
			Debug.LogError($"Upgrade Tool slot -> Show info about where to find resource '<color=yellow>{_resourceId}</color>'");
		}

		#endregion
	}
}