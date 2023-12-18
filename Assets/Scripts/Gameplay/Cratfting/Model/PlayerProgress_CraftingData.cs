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
		private PlayerProgress_CraftingAreaData[] areas;

		public void Initialize()
		{
			areas = new PlayerProgress_CraftingAreaData[10];
		}

		public CraftingStatus GetAreaStatus(int id)
		{
			// TODO
			return CraftingStatus.NONE;
		}

		public void StartProcess(int id, float expiration)
		{
			// TODO:
			// - check if it is in status: NONE, else skip it
			// - consume resources required
			// - persist area id
			// - persist expiration time
			// - persist area status: in progress
		}

		public void CompleteProcess(int id)
		{
			// TODO:
			// - persist area status: completed
		}

		public void Claim(int id)
		{
			// TODO:
			// - check status: COMPLETED
			// - grant resources obtained
			// - remove this area from persisted data
		}
	}
}