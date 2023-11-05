/*
 * Date: November 1st, 2023
 * Author: Peche
 */

using LittleLooters.General;
using System;

namespace LittleLooters.Model
{
    [Serializable]
    public struct PlayerResourceData
	{
        public ResourceData info;
        public int amount;
    }

    [Serializable]
    public struct PlayerProgress_ResourcesData
    {
        public PlayerResourceData[] resources;

        public void Grant(int id, int amount)
		{
            var index = GetIndexById(id);

            resources[index].amount += amount;

            var updatedAmount = resources[index].amount;

            PlayerProgressEvents.OnResourceHasChanged?.Invoke(id, updatedAmount);
		}


        private int GetIndexById(int id)
		{
			for (int i = 0; i < resources.Length; i++)
			{
                if (resources[i].info.Id == id) return i;
			}

            return 0;
		}
    }
}