/*
 * Date: April 13th, 2024
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
	public class UI_GameplayCheats_SlotSwipe : MonoBehaviour
	{
		#region Inspector

		[SerializeField] private Button _btnApply = default;
		[SerializeField] private UI_GameplayCheats_SwipePanel _panel = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtTitle = default;

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
			_panel.Show(HidePanel);

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