/*
 * Date: November 15th, 2023
 * Author: Peche
 */

using System;

namespace LittleLooters.Gameplay
{
    public class PlayerMissionsEvents
    {
        public static Action<MissionConfigurationData> OnInitialization;

        public static Action<MissionConfigurationData> OnMoveToMission;

        public static Action<int, int> OnMissionProgress;
    }
}