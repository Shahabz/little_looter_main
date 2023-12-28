/*
 * Date: December 14th, 2023
 * Author: Peche
 */

using System;

namespace LittleLooters.Model
{
	[Serializable]
	public struct PlayerProgress_CraftingData
	{
		public PlayerProgress_CraftingAreaData[] areas;

		public void Initialize()
		{
			areas = new PlayerProgress_CraftingAreaData[10];

			for (int i = 0; i < areas.Length; i++)
			{
				areas[i].id = -1;
			}
		}

		public CraftingStatus GetAreaStatus(int id)
		{
			for (int i = 0; i < areas.Length; i++)
			{
				var areaProgressData = areas[i];

				if (areaProgressData.id != id) continue;

				return areaProgressData.status;
			}

			return CraftingStatus.NONE;
		}

		public void StartProcess(int id, int durationByUnit, int amount, float now)
		{
			var totalDuration = durationByUnit * amount;
			var expiration = now + totalDuration;

			var areaProgressData = new PlayerProgress_CraftingAreaData() { 
				id = id,
				expiration = expiration,
				amount = amount,
				status = CraftingStatus.IN_PROGRESS
			};

			var index = GetFreeSpace();

			areas[index] = areaProgressData;

			PlayerProgressEvents.OnCraftingAreaProcessStarted?.Invoke(areaProgressData);
		}

		public void CompleteProcess(int id)
		{
			for (int i = 0; i < areas.Length; i++)
			{
				var areaProgressData = areas[i];

				if (areaProgressData.id != id) continue;

				areaProgressData.status = CraftingStatus.COMPLETED;
				areaProgressData.expiration = 0;

				areas[i] = areaProgressData;

				PlayerProgressEvents.OnCraftingAreaProcessCompleted?.Invoke(areaProgressData);

				break;
			}
		}

		public PlayerProgress_CraftingAreaData GetAreaProgressData(int id)
		{
			for (int i = 0; i < areas.Length; i++)
			{
				var areaProgressData = areas[i];

				if (areaProgressData.id != id) continue;

				return areaProgressData;
			}

			return areas[0];
		}

		public void ClaimProcess(int id)
		{
			for (int i = 0; i < areas.Length; i++)
			{
				var areaProgressData = areas[i];

				if (areaProgressData.id != id) continue;

				PlayerProgressEvents.OnCraftingAreaProcessClaimed?.Invoke(areaProgressData);
				
				// Clear data
				areaProgressData.status = CraftingStatus.NONE;
				areaProgressData.expiration = 0;
				areaProgressData.amount = 0;
				areaProgressData.id = -1;

				// Refresh data
				areas[i] = areaProgressData;

				break;
			}
		}

		public void SpeedUpProcess(int id, int seconds, float now)
		{
			var wasCompleted = false;

			for (int i = 0; i < areas.Length; i++)
			{
				var areaProgressData = areas[i];

				if (areaProgressData.id != id) continue;

				areaProgressData.expiration -= seconds;

				// Check if process was completed
				if (areaProgressData.expiration <= now)
				{
					areaProgressData.status = CraftingStatus.COMPLETED;
					areaProgressData.expiration = 0;
					wasCompleted = true;
				}

				areas[i] = areaProgressData;

				if (wasCompleted)
				{
					PlayerProgressEvents.OnCraftingAreaProcessCompleted?.Invoke(areaProgressData);
				}
				else
				{
					PlayerProgressEvents.OnCraftingAreaProcessSpeedUp?.Invoke(areaProgressData);
				}

				break;
			}
		}

		private int GetFreeSpace()
		{
			for (int i = 0; i < areas.Length; i++)
			{
				if (areas[i].id == -1) return i;
			}

			return 0;
		}
	}
}