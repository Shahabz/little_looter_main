/*
 * Date: January 14th, 2024
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;
using System;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_CurrentMission_FtuePanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _panel = default;
		[SerializeField] private VariableJoystick _joystick = default;
		[SerializeField] private PlayerEntryPoint _playerEntryPoint = default;
		[SerializeField] private UI_CurrentMissionPanel _missionPanel = default;
		[SerializeField] private GameObject _infoPanel = default;
		[SerializeField] private GameObject _background = default;

		#endregion

		#region Private properties

		private Canvas _canvas = default;
		private int _remainingTimes = 3;
		private bool _showJoystick = true;

		#endregion

		#region Unity events

		private void Awake()
		{
			_canvas = GetComponent<Canvas>();

			UI_GameplayEvents.OnTriggerMissionAssistance += HandleTriggerMissionAssistance;
			PlayerProgressEvents.OnMoveToNextMission += HandleMoveToNextMission;

			_joystick.OnChange += HandleJoystickMovement;
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnTriggerMissionAssistance -= HandleTriggerMissionAssistance;
			PlayerProgressEvents.OnMoveToNextMission -= HandleMoveToNextMission;

			_joystick.OnChange -= HandleJoystickMovement;
		}

		#endregion

		#region Private methods

		private void HandleJoystickMovement(Vector2 obj)
		{
			_joystick.OnChange -= HandleJoystickMovement;

			Invoke(nameof(ShowPanelWithDelay), 1);
		}

		private void ShowPanelWithDelay()
		{
			_joystick.Stop();
			_joystick.gameObject.SetActive(false);

			ShowPanel();
		}

		private void HandleTriggerMissionAssistance()
		{

			if (_showJoystick)
			{
				_joystick.gameObject.SetActive(true);
				_showJoystick = false;
			}

			Hide();
		}

		private void HandleMoveToNextMission()
		{
			_remainingTimes--;

			Invoke(nameof(ShowFtueIndicator), 2f);
		}

		private void ShowFtueIndicator()
		{
			_missionPanel.SetFtueCanvasOrder(_canvas.sortingOrder + 5);

			_panel.SetActive(true);

			_infoPanel.SetActive(false);
			_background.SetActive(false);
		}

		private void Hide()
		{
			_panel.SetActive(false);

			_missionPanel.SetOriginalCanvasOrder();

			if (_remainingTimes > 0) return;

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

		#endregion
	}
}