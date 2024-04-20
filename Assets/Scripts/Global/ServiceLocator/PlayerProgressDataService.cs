/*
 * Date: January 20th, 2024
 * Author: Peche
 */

using LittleLooters.Gameplay;
using LittleLooters.General;
using LittleLooters.Model;
using UnityEngine;

namespace LittleLooters.Global.ServiceLocator
{
    public class PlayerProgressDataService : MonoBehaviour, IGameService
    {
		#region Inspector

		[SerializeField] private PlayerProgressData _progressData = default;

		#endregion

		#region Public properties

		public PlayerProgressData ProgressData => _progressData;

		#endregion

		#region Unity events

		private void Awake()
		{
			SubscribeEvents();
		}

		private void OnDestroy()
		{
			UnsubscribeEvents();
		}

		#endregion

		#region Private methods

		private void SubscribeEvents()
		{
			UI_GameplayEvents.OnStartGame += HandleStartGame;

			Tool_SubscribeEvents();

			Repair_SubscribeEvents();

			Missions_SubscribeEvents();

			Cheats_SubscribeEvents();
		}

		private void UnsubscribeEvents()
		{
			UI_GameplayEvents.OnStartGame -= HandleStartGame;

			Tool_UnsubscribeEvents();

			Repair_UnsubscribeEvents();

			Missions_UnsubscribeEvents();

			Cheats_UnsubscribeEvents();
		}

		private void HandleStartGame()
		{
			Tool_InitializeData();

			Resources_Initialization();
		}

		#endregion

		#region Resources

		public void Resources_Initialization()
		{
			var gameConfiguration = ServiceLocator.Current.Get<GameConfigurationService>();

			_progressData.InitializeResources(gameConfiguration.Resources);
		}

		public void Resources_GrantByDestructionDamage(int id, int amount)
		{
			_progressData.GrantResourceAmount(id, amount);
		}

		public void Resources_GrantByPickup(int id, int amount)
		{
			_progressData.GrantResourceAmount(id, amount);
		}

		private void Resources_Grant(int id, int amount)
		{
			_progressData.GrantResourceAmount(id, amount);
		}

		private void Resources_Consume(int id, int amount)
		{
			_progressData.ConsumeResourceAmount(id, amount);
		}

		#endregion

		#region Tool

		private void Tool_InitializeData()
		{
			var gameConfiguration = ServiceLocator.Current.Get<GameConfigurationService>();

			_progressData.SetMeleeData(gameConfiguration.ToolLevels[0]);
		}

		public ConfigurationMeleeLevelData Tool_GetNextLevelData()
		{
			var gameConfiguration = ServiceLocator.Current.Get<GameConfigurationService>();

			var toolLevels = gameConfiguration.ToolLevels;

			var currentLevel = _progressData.toolData.level;
			var nextLevelData = toolLevels[0];

			for (int i = 0; i < toolLevels.Length; i++)
			{
				nextLevelData = toolLevels[i];

				if (nextLevelData.level != currentLevel + 1) continue;

				break;
			}

			return nextLevelData;
		}

		public ConfigurationMeleeLevelData Tool_GetCurrentLevelData()
		{
			var gameConfiguration = ServiceLocator.Current.Get<GameConfigurationService>();

			var toolLevels = gameConfiguration.ToolLevels;

			var currentLevel = _progressData.toolData.level;
			var levelData = toolLevels[0];

			for (int i = 0; i < toolLevels.Length; i++)
			{
				levelData = toolLevels[i];

				if (levelData.level != currentLevel) continue;

				break;
			}

			return levelData;
		}

		private void Tool_SubscribeEvents()
		{
			UI_GameplayEvents.OnStartToolUpgrade += HandleToolStartUpgrade;
			UI_GameplayEvents.OnClaimToolUpgrade += HandleToolUpgradeClaim;
			UI_GameplayEvents.OnSpeedUpToolUpgrade += HandleToolSpeedUpUpgrade;
			UI_GameplayEvents.OnIncreaseToolDamage += HandleToolIncreaseDamage;
			UI_GameplayEvents.OnToolUpgradeByCheat += HandleToolUpgradeByCheat;
		}

		private void Tool_UnsubscribeEvents()
		{
			UI_GameplayEvents.OnStartToolUpgrade -= HandleToolStartUpgrade;
			UI_GameplayEvents.OnClaimToolUpgrade -= HandleToolUpgradeClaim;
			UI_GameplayEvents.OnSpeedUpToolUpgrade -= HandleToolSpeedUpUpgrade;
			UI_GameplayEvents.OnIncreaseToolDamage -= HandleToolIncreaseDamage;
			UI_GameplayEvents.OnToolUpgradeByCheat -= HandleToolUpgradeByCheat;
		}

		private void HandleToolStartUpgrade()
		{
			Tool_StartUpgrade();
		}

		private void HandleToolUpgradeClaim()
		{
			Tool_ClaimUpgrade();
		}

		private void HandleToolSpeedUpUpgrade()
		{
			Tool_SpeedUpUpgrade();
		}

		private void HandleToolIncreaseDamage()
		{
			Tool_IncreaseDamage();
		}

		private void HandleToolUpgradeByCheat()
		{
			Tool_ClaimUpgrade();
		}

		public void Tool_CompleteExtraDamage()
		{
			var toolLevelData = Tool_GetCurrentLevelData();

			_progressData.CompleteIncreaseToolDamage(toolLevelData);
		}

		public void Tool_CompleteUpgrade()
		{
			_progressData.CompleteMeleeUpgrade();
		}

		private void Tool_StartUpgrade()
		{
			var nextLevelData = Tool_GetNextLevelData();
			var now = Time.time;
			var duration = nextLevelData.upgradeTime;
			var expiration = now + duration;

			_progressData.StartMeleeUpgrade(duration, expiration, nextLevelData.requirements, nextLevelData.damage);
		}

		private void Tool_ClaimUpgrade()
		{
			var nextLevelData = Tool_GetNextLevelData();

			_progressData.ClaimMeleeUpgrade(nextLevelData.damage);
		}

		private void Tool_SpeedUpUpgrade()
		{
			_progressData.SpeedUpToolUpgrade();
		}

		private void Tool_IncreaseDamage()
		{
			var now = Time.time;

			var gameConfiguration = ServiceLocator.Current.Get<GameConfigurationService>();

			var duration = gameConfiguration.ToolExtraDamageDuration;

			_progressData.IncreaseToolDamage(now, duration);
		}

		#endregion

		#region Crafting

		public void CraftingInitialization()
		{
			_progressData.InitCrafting();
		}

		public PlayerProgress_CraftingAreaData[] CraftingGetAreas()
		{
			return _progressData.craftingData.areas;
		}

		public void CraftingStartProcess(CraftingConfigurationData data, int amount)
		{
			_progressData.CraftingStartProcess(data, amount);
		}

		public void CraftingCompleteProcess(int areaId)
		{
			_progressData.CraftingCompleteProcess(areaId);
		}

		public int CraftingClaimProcess(int resourceId, int areaId)
		{
			var amountObtained = _progressData.CraftingClaimProcess(areaId, resourceId);

			return amountObtained;
		}

		public void CraftingSpeedUpProcess(int areaId, int secondsToSkip)
		{
			var now = Time.time;

			_progressData.CraftingSpeedUpProcess(areaId, secondsToSkip, now);
		}

		#endregion

		#region Repair

		public void Repair_Setup(RepairObject[] repairObjects)
		{
			_progressData.InitRepairProgress(repairObjects);
		}

		public void Repair_CompleteProcess(int id)
		{
			_progressData.CompleteRepairing(id);
		}

		private void Repair_SubscribeEvents()
		{
			UI_GameplayEvents.OnFixSlot += HandleFixSlot;
			UI_GameplayEvents.OnStartRepairing += HandleStartRepairing;
			UI_GameplayEvents.OnSpeedUpRepairing += HandleSpeedUpRepairing;
		}

		private void Repair_UnsubscribeEvents()
		{
			UI_GameplayEvents.OnFixSlot -= HandleFixSlot;
			UI_GameplayEvents.OnStartRepairing -= HandleStartRepairing;
			UI_GameplayEvents.OnSpeedUpRepairing -= HandleSpeedUpRepairing;
		}

		private void HandleFixSlot(int objectId, int resourceId)
		{
			Repair_FixSlot(objectId, resourceId);
		}

		private void HandleStartRepairing(int id)
		{
			Repair_StartProcess(id);
		}

		private void HandleSpeedUpRepairing(int objectId)
		{
			Repair_SpeedUpProcess(objectId);
		}

		private void Repair_FixSlot(int objectId, int resourceId)
		{
			_progressData.Fix(objectId, resourceId);
		}

		private void Repair_StartProcess(int id)
		{
			var now = Time.time;

			_progressData.StartRepairing(id, now);
		}

		private void Repair_SpeedUpProcess(int id)
		{
			_progressData.SpeedUpRepairing(id);
		}

		#endregion

		#region Missions

		private void Missions_SubscribeEvents()
		{
			PlayerMissionsEvents.OnMoveToMission += HandleMoveToMission;
		}

		private void Missions_UnsubscribeEvents()
		{
			PlayerMissionsEvents.OnMoveToMission -= HandleMoveToMission;
		}

		private void HandleMoveToMission(MissionConfigurationData missionData)
		{
			Missions_UpdateCurrentMission(missionData.Id);
		}

		private void Missions_UpdateCurrentMission(int id)
		{
			_progressData.UpdateCurrentMission(id);
		}

		#endregion

		#region Cheats

		private void Cheats_SubscribeEvents()
		{
			UI_GameplayEvents.OnGrantResourceByCheat += HandleGrantResourceByCheat;
			UI_GameplayEvents.OnConsumeResourceByCheat += HandleConsumeResourceByCheat;
		}

		private void Cheats_UnsubscribeEvents()
		{
			UI_GameplayEvents.OnGrantResourceByCheat -= HandleGrantResourceByCheat;
			UI_GameplayEvents.OnConsumeResourceByCheat -= HandleConsumeResourceByCheat;
		}

		private void HandleGrantResourceByCheat(UI_GameplayEvents.UpdateResourceByCheatArgs args)
		{
			Resources_Grant(args.resourceId, args.amount);
		}

		private void HandleConsumeResourceByCheat(UI_GameplayEvents.UpdateResourceByCheatArgs args)
		{
			Resources_Consume(args.resourceId, args.amount);
		}

		#endregion
	}
}