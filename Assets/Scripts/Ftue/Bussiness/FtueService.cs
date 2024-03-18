/*
 * Date: March 18th, 2024
 * Author: Peche
 */

using LittleLooters.Ftue.View;
using LittleLooters.Gameplay;
using UnityEngine;

namespace LittleLooters.Ftue
{
    public class FtueService : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private FtueSteps _step = FtueSteps.NONE;
		[SerializeField] private UI_Ftue_MissionsPanel _missionsPanel = default;
		[SerializeField] private UI_Ftue_MovementPanel _movementPanel = default;
		[SerializeField] private UI_Ftue_ResourcesPanel _resourcesPanel = default;
		[SerializeField] private UI_Ftue_ToolInfoPanel _toolInfoPanel = default;
		[SerializeField] private UI_Ftue_BaseTrigger _triggerMovement = default;
		[SerializeField] private UI_Ftue_BaseTrigger _triggerDestruction = default;
		[SerializeField] private UI_Ftue_BaseTrigger _triggerDestructionCompleted = default;
		[SerializeField] private UI_Ftue_BaseTrigger _triggerDestructionSecondTime = default;
		[SerializeField] private UI_Ftue_BaseTrigger _triggerDestructionSecondTimeCompleted = default;
		[SerializeField] private UI_Ftue_BaseTrigger _triggerKillEnemy = default;
		[SerializeField] private UI_Ftue_BaseTrigger _triggerKillEnemyCompleted = default;
		[SerializeField] private UI_Ftue_BaseTrigger _triggerUpgradeTool = default;
		[SerializeField] private UI_Ftue_BaseTrigger _triggerDestructionDoor = default;
		[SerializeField] private UI_Ftue_BaseTrigger _triggerDestructionDoorCompleted = default;
		[SerializeField] private UI_Ftue_messages _floatingMessagesPanel = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			UI_GameplayEvents.OnStartGame += HandleGameStarted;

			_triggerMovement.OnChanged += HandleTriggerMovementChanged;
			_triggerDestruction.OnChanged += HandleTriggerDestructionChanged;
			_triggerDestructionCompleted.OnChanged += HandleTriggerDestructionCompletedChanged;
			_triggerDestructionSecondTime.OnChanged += HandleTriggerDestructionSecondTimeChanged;
			_triggerDestructionSecondTimeCompleted.OnChanged += HandleTriggerDestructionSecondTimeCompletedChanged;
			_triggerUpgradeTool.OnChanged += HandleTriggerUpgradeToolChanged;
			_triggerKillEnemy.OnChanged += HandleTriggerKillEnemyChanged;
			_triggerKillEnemyCompleted.OnChanged += HandleTriggerKillEnemyCompletedChanged;
			_triggerDestructionDoor.OnChanged += HandleTriggerDestructionDoorChanged;
			_triggerDestructionDoorCompleted.OnChanged += HandleTriggerDestructionDoorCompletedChanged;
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnStartGame -= HandleGameStarted;

			_triggerMovement.OnChanged -= HandleTriggerMovementChanged;
			_triggerDestruction.OnChanged -= HandleTriggerDestructionChanged;
			_triggerDestructionCompleted.OnChanged -= HandleTriggerDestructionCompletedChanged;
			_triggerDestructionSecondTime.OnChanged -= HandleTriggerDestructionSecondTimeChanged;
			_triggerDestructionSecondTimeCompleted.OnChanged -= HandleTriggerDestructionSecondTimeCompletedChanged;
			_triggerUpgradeTool.OnChanged -= HandleTriggerUpgradeToolChanged;
			_triggerKillEnemy.OnChanged -= HandleTriggerKillEnemyChanged;
			_triggerKillEnemyCompleted.OnChanged -= HandleTriggerKillEnemyCompletedChanged;
			_triggerDestructionDoor.OnChanged -= HandleTriggerDestructionDoorChanged;
			_triggerDestructionDoorCompleted.OnChanged -= HandleTriggerDestructionDoorCompletedChanged;
		}

		#endregion

		#region Private methods

		private void HandleGameStarted()
		{
			_step = FtueSteps.GAME_STARTED;

			_missionsPanel.Hide();
			_movementPanel.Show();
			_resourcesPanel.Hide();
			_toolInfoPanel.Hide();

			DebugStepChanged();
		}

		private void HandleTriggerMovementChanged(bool status)
		{
			if (_step == FtueSteps.GAME_STARTED)
			{
				_step = FtueSteps.FIRST_MOVEMENT;
				DebugStepChanged();
			}

			if (status) _floatingMessagesPanel.ShowMovementMessage();
		}

		private void HandleTriggerDestructionChanged(bool status)
		{
			if (_step != FtueSteps.FIRST_MOVEMENT) return;

			if (status)
			{
				_floatingMessagesPanel.ShowDestructionMessage();
				return;
			}

			_floatingMessagesPanel.HideAll();
		}

		private void HandleTriggerDestructionCompletedChanged(bool status)
		{
			if (!status) return;

			if (_step != FtueSteps.FIRST_MOVEMENT) return;

			_step = FtueSteps.FIRST_DESTRUCTIBLE;

			_floatingMessagesPanel.HideAll();

			DebugStepChanged();
		}

		private void HandleTriggerDestructionSecondTimeChanged(bool status)
		{
			if (_step != FtueSteps.FIRST_DESTRUCTIBLE) return;

			if (status)
			{
				_floatingMessagesPanel.ShowDestructionMessage();
				return;
			}

			_floatingMessagesPanel.HideAll();
		}

		private void HandleTriggerDestructionSecondTimeCompletedChanged(bool status)
		{
			if (!status) return;

			if (_step != FtueSteps.FIRST_DESTRUCTIBLE) return;

			_step = FtueSteps.SECOND_DESTRUCTIBLE;

			_floatingMessagesPanel.HideAll();

			DebugStepChanged();
		}

		private void HandleTriggerUpgradeToolChanged(bool status)
		{
			if (!status) return;

			if (_step != FtueSteps.FIRST_DESTRUCTIBLE) return;

			_step = FtueSteps.FIRST_TOOL_UPGRADE;

			DebugStepChanged();
		}

		private void HandleTriggerKillEnemyChanged(bool status)
		{
			if (!status) return;

			if (_step != FtueSteps.SECOND_DESTRUCTIBLE) return;

			_step = FtueSteps.FIRST_ENEMY;

			_floatingMessagesPanel.ShowKillEnemyMessage();

			DebugStepChanged();
		}

		private void HandleTriggerKillEnemyCompletedChanged(bool status)
		{
			if (!status) return;

			if (_step != FtueSteps.FIRST_ENEMY) return;

			_step = FtueSteps.DOOR_DESTRUCTIBLE;

			_floatingMessagesPanel.HideAll();

			DebugStepChanged();
		}

		private void HandleTriggerDestructionDoorChanged(bool status)
		{
			if (_step != FtueSteps.DOOR_DESTRUCTIBLE) return;

			if (status)
			{
				_floatingMessagesPanel.ShowDestructionMessage();
				return;
			}

			_floatingMessagesPanel.HideAll();
		}

		private void HandleTriggerDestructionDoorCompletedChanged(bool status)
		{
			if (!status) return;

			if (_step != FtueSteps.DOOR_DESTRUCTIBLE) return;

			_step = FtueSteps.FIRST_TOOL_UPGRADE;

			_floatingMessagesPanel.HideAll();

			DebugStepChanged();
		}

		#endregion

		#region Debug

		private void DebugStepChanged()
		{
			Debug.LogError($"STEP: <color=orange>{_step}</color>");
		}

		#endregion
	}
}