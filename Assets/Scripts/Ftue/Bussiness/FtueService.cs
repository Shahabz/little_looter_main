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
		[SerializeField] private EnemyController[] _enemies = default;
		[SerializeField] private DestructibleResourceObject[] _firstBushes = default;
		[SerializeField] private DestructibleResourceObject[] _secondBushes = default;

		#endregion

		#region Private properties

		private int _firstRemainingBushes = 0;
		private int _secondRemainingBushes = 0;
		private int _remainingEnemies = 0;
		private int _detectedEnemies = 0;

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
			_triggerDestructionDoor.OnChanged += HandleTriggerDestructionDoorChanged;
			_triggerDestructionDoorCompleted.OnChanged += HandleTriggerDestructionDoorCompletedChanged;

			// First bushes
			_firstRemainingBushes = _firstBushes.Length;

			for (int i = 0; i < _firstRemainingBushes; i++)
			{
				var bush = _firstBushes[i];
				bush.OnDead += HandleFirstBushesDestruction;
			}

			// Second bushes
			_secondRemainingBushes = _secondBushes.Length;

			for (int i = 0; i < _secondRemainingBushes; i++)
			{
				var bush = _secondBushes[i];
				bush.OnDead += HandleSecondBushesDestruction;
			}

			// Enemies
			_remainingEnemies = _enemies.Length;

			for (int i = 0; i < _remainingEnemies; i++)
			{
				var enemy = _enemies[i];
				enemy.OnDead += HandleEnemyDeath;
				enemy.OnWasDetected += HandleEnemyStartDetection;
				enemy.OnDetectionFinished += HandleEnemyStopDetection;
			}
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
			_triggerDestructionDoor.OnChanged -= HandleTriggerDestructionDoorChanged;
			_triggerDestructionDoorCompleted.OnChanged -= HandleTriggerDestructionDoorCompletedChanged;

			// First bushes
			for (int i = 0; i < _firstBushes.Length; i++)
			{
				var bush = _firstBushes[i];
				bush.OnDead += HandleFirstBushesDestruction;
			}

			// Second bushes
			for (int i = 0; i < _secondBushes.Length; i++)
			{
				var bush = _secondBushes[i];
				bush.OnDead += HandleSecondBushesDestruction;
			}

			// Enemies
			for (int i = 0; i < _enemies.Length; i++)
			{
				var enemy = _enemies[i];
				enemy.OnDead -= HandleEnemyDeath;
				enemy.OnWasDetected -= HandleEnemyStartDetection;
				enemy.OnDetectionFinished -= HandleEnemyStopDetection;
			}
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

			_floatingMessagesPanel.ShowMovementMessage();

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

			_missionsPanel.Show();

			_toolInfoPanel.Show();

			_floatingMessagesPanel.HideAll();

			DebugStepChanged();
		}

		private void HandleEnemyStartDetection()
		{
			if ((int)_step < (int)FtueSteps.SECOND_DESTRUCTIBLE) return;

			if (_step == FtueSteps.SECOND_DESTRUCTIBLE)
			{
				_step = FtueSteps.FIRST_ENEMY;
				DebugStepChanged();
			}

			_detectedEnemies++;

			_floatingMessagesPanel.ShowKillEnemyMessage();
		}

		private void HandleEnemyStopDetection()
		{
			if (_remainingEnemies <= 0) return;

			if ((int)_step <= (int)FtueSteps.SECOND_DESTRUCTIBLE) return;

			_detectedEnemies--;

			if (_detectedEnemies > 0) return;

			_floatingMessagesPanel.HideAll();
		}

		private void HandleEnemyDeath()
		{
			_remainingEnemies--;

			if (_remainingEnemies == 2)
			{
				_step = FtueSteps.SECOND_ENEMY;
				DebugStepChanged();
			}
			else if (_remainingEnemies == 1)
			{
				_step = FtueSteps.THIRD_ENEMY;
				DebugStepChanged();
			}

			if (_remainingEnemies > 0) return;

			_step = FtueSteps.DOOR_DESTRUCTIBLE;

			_floatingMessagesPanel.ShowDoorMessage();

			DebugStepChanged();
		}

		private void HandleFirstBushesDestruction()
		{
			_firstRemainingBushes--;

			if (_firstRemainingBushes == 1)
			{
				_resourcesPanel.Show();
			}

			if (_firstRemainingBushes > 0) return;

			_floatingMessagesPanel.HideAll();
		}

		private void HandleSecondBushesDestruction()
		{
			_secondRemainingBushes--;

			if (_secondRemainingBushes > 0) return;

			_floatingMessagesPanel.HideAll();
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