/*
 * Date: November 18th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class MissionUpgradeToolAssistance : MonoBehaviour
    {
        public (Transform target, Vector3 targetOffset, bool found) Process(MissionConfigurationData mission, PlayerProgressData playerProgressData)
        {
            Debug.LogError($"<color=magenta>UPGRADE TOOL</color> -> Current mission <color=yellow>'{mission.Description}'</color>, type: <color=orange>{mission.Type}</color>");

            Transform target = null;
            Vector3 offset = Vector3.zero;
            var found = false;

            return (target, offset, found);
        }
    }
}