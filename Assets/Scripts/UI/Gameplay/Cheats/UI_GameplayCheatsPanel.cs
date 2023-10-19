/*
 * Date: October 19th, 2023
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_GameplayCheatsPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private Button _btnAccess = default;
        [SerializeField] private GameObject _panel = default;
        [SerializeField] private Button _btnClosePanel = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			Init();
		}

		private void OnDestroy()
		{
			Teardown();
		}

		#endregion

		#region Private methods

		private void Init()
		{
			_btnAccess.onClick.AddListener(OpenPanel);
			_btnClosePanel.onClick.AddListener(ClosePanel);
		}

		private void Teardown()
		{
			_btnAccess.onClick.RemoveAllListeners();
			_btnClosePanel.onClick.RemoveAllListeners();
		}

		private void OpenPanel()
		{
			_panel.SetActive(true);
		}

		private void ClosePanel()
		{
			_panel.SetActive(false);
		}

		#endregion
	}
}