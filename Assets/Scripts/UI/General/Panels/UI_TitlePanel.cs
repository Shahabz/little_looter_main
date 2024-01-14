/*
 * Date: January 14th, 2024
 * Author: Peche
 */

using LittleLooters.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.General.UI
{
	public class UI_TitlePanel : MonoBehaviour
	{
		[SerializeField] private GameObject _panel = default;
		[SerializeField] private Button _btnAccess = default;

		private void Awake()
		{
			_btnAccess.onClick.AddListener(ClosePanel);

			ShowPanel();
		}

		private void OnDestroy()
		{
			_btnAccess.onClick.RemoveAllListeners();
		}

		private void ClosePanel()
		{
			// TODO: SFX

			HidePanel();

			UI_GameplayEvents.OnStartGame?.Invoke();
		}

		private void ShowPanel()
		{
			_panel.SetActive(true);
		}

		private void HidePanel()
		{
			_panel.SetActive(false);
		}
	}
}