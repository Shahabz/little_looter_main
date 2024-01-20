/*
 * Date: December 17th, 2023
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_GameplayCheats_SlotResources : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private PlayerEntryPoint _playerEntryPoint = default;
        [SerializeField] private Button _btnApply = default;
        [SerializeField] private UI_GameplayCheats_ResourcesPanel _panel = default;
		[SerializeField] private TextMeshProUGUI _txtTitle = default;

		#endregion

		#region Unity events

		private void OnEnable()
		{
			_btnApply.onClick.AddListener(Apply);

			HidePanel();
		}

		private void OnDisable()
		{
			_btnApply.onClick.RemoveAllListeners();
		}

		#endregion

		#region Private methods

		private void Apply()
		{
			var isOpen = _panel.IsOpen;

			if (isOpen)
			{
				HidePanel();
				return;
			}

			ShowPanel();
		}

		private void ShowPanel()
		{
			var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();

			_panel.Show(progressDataService.ProgressData);

			_txtTitle.text = "HIDE";
		}

		private void HidePanel()
		{
			_panel.Hide();
			_txtTitle.text = "SHOW";
		}

		#endregion
	}
}