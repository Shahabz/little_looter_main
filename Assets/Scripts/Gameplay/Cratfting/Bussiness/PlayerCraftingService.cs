/*
 * Date: December 14th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class PlayerCraftingService : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private CraftingArea[] _areas = default;

		#endregion

		#region Private properties

		private PlayerEntryPoint _entryPoint = default;
		private bool _isDead = false;
		private const string _tag = "Crafting";

		#endregion

		#region Unity events

		private void OnTriggerEnter(Collider other)
		{
			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<CraftingArea>(out var craftingArea)) return;

			StartInteraction(craftingArea);
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<CraftingArea>(out var craftingArea)) return;

			StopInteraction(craftingArea);
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnCraftingStarted -= HandleOnCraftingStarted;
			UI_GameplayEvents.OnCraftingClaimed -= HandleOnCraftingClaimed;
			UI_GameplayEvents.OnCraftingSpeedUp -= HandleOnCraftingSpeedUp;

			if (_entryPoint != null)
			{
				_entryPoint.Health.OnDead -= HandleOnDead;
			}
		}

		private void Update()
		{
			if (_isDead) return;

			CheckCraftingProgress();
		}

		#endregion

		#region Public methods

		public void Initialization(PlayerEntryPoint entryPoint)
		{
			UI_GameplayEvents.OnCraftingStarted += HandleOnCraftingStarted;
			UI_GameplayEvents.OnCraftingClaimed += HandleOnCraftingClaimed;
			UI_GameplayEvents.OnCraftingSpeedUp += HandleOnCraftingSpeedUp;

			_entryPoint = entryPoint;

			_entryPoint.Health.OnDead += HandleOnDead;
		}

		public CraftingConfigurationData GetConfigurationAreaData(int id)
		{
			for (int i = 0; i < _areas.Length; i++)
			{
				var area = _areas[i];

				if (area.Id == id) return area.Data;
			}

			return _areas[0].Data;
		}

		#endregion

		#region Private methods

		private void StartInteraction(CraftingArea area)
		{
			area.StartInteraction();
		}

		private void StopInteraction(CraftingArea area)
		{
			area.StopInteraction();
		}

		private void HandleOnDead()
		{
			_isDead = true;
		}

		private void HandleOnCraftingStarted(int areaId, int amount)
		{
			var data = GetConfigurationAreaData(areaId);

			_entryPoint.CraftingStartProcess(data, amount);
		}

		private void CheckCraftingProgress()
		{
			var now = Time.time;
			var craftingAreas = _entryPoint.ProgressData.craftingData.areas;

			// Check if some crafting in progress was completed
			for (int i = 0; i < craftingAreas.Length; i++)
			{
				var area = craftingAreas[i];

				if (area.status != Model.CraftingStatus.IN_PROGRESS) continue;

				if (area.expiration > now) continue;

				_entryPoint.CraftingCompleteProcess(area.id);
			}
		}

		private void HandleOnCraftingClaimed(int areaId)
		{
			var configurationData = GetConfigurationAreaData(areaId);
			var newResourceId = configurationData.ResourceGenerated.Id;

			var amountObtained = _entryPoint.CraftingClaimProcess(newResourceId, areaId);

			UI.UI_ResourcesAnimation.OnAnimate?.Invoke(newResourceId, amountObtained);
		}

		private void HandleOnCraftingSpeedUp(int areaId, int seconds)
		{
			_entryPoint.CraftingSpeedUpProcess(areaId, seconds);
		}

		#endregion
	}
}