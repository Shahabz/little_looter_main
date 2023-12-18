/*
 * Date: December 16th, 2023
 * Author: Peche
 */

using System;

namespace LittleLooters.Model
{
	[Serializable]
	public struct PlayerProgress_CraftingAreaData
	{
		public int id;
		public float expiration;
		public CraftingStatus status;
	}
}