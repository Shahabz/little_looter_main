/*
 * Date: November 15th, 2023
 * Author: Peche
 */

using System;

namespace LittleLooters.Model
{
    [Serializable]
    public struct PlayerProgress_MissionsData
    {
        private int currentMissionId;

        public int GetCurrentMissionId()
		{
            return currentMissionId;
		}

        public void UpdateCurrentMission(int id)
		{
            currentMissionId = id;

            PlayerProgressEvents.OnMoveToNextMission?.Invoke();
		}
    }
}