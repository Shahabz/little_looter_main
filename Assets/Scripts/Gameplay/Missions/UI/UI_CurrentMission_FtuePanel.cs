/*
 * Date: January 14th, 2024
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_CurrentMission_FtuePanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _panel = default;
		[SerializeField] private PlayerEntryPoint _playerEntryPoint = default;
		[SerializeField] private UI_CurrentMissionPanel _missionPanel = default;

		#endregion

		#region Private properties

		private Canvas _canvas = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			_canvas = GetComponent<Canvas>();

			UI_GameplayEvents.OnPlayerInitialization += HandlePlayerInitialization;
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnPlayerInitialization -= HandlePlayerInitialization;
			UI_GameplayEvents.OnTriggerMissionAssistance -= HandleTriggerMissionAssistance;
		}

		#endregion

		private void HandlePlayerInitialization()
		{
			var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();

			// Check current player's mission
			var currentMissionId = progressDataService.ProgressData.missionsData.GetCurrentMissionId();

			var firstMission = _playerEntryPoint.GetFirstMissionData();

			if (currentMissionId != firstMission.Id)
			{
				DestroyPanel();

				return;
			}

			UI_GameplayEvents.OnTriggerMissionAssistance += HandleTriggerMissionAssistance;

			ShowPanel();
		}

		private void HandleTriggerMissionAssistance()
		{
			DestroyPanel();
		}

		private void DestroyPanel()
		{
			_panel.SetActive(false);

			_missionPanel.SetOriginalCanvasOrder();

			Destroy(gameObject);
		}

		private void ShowPanel()
		{
			UI_GameplayEvents.OnStopMissionAssistance?.Invoke();

			_missionPanel.SetFtueCanvasOrder(_canvas.sortingOrder + 5);

			_panel.SetActive(true);
		}
	}
}