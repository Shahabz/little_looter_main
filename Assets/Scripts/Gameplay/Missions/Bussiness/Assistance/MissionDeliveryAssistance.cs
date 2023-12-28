/*
 * Date: December 27th, 2023
 * Author: Peche
 */

using System.Collections.Generic;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    /// <summary>
    /// Logic related with delivery mission assistance.
    /// When it is triggered, it tries to detect the repairable object related with the mission.
    /// If it is on screen, then the indicator should be shown, else, the offscreen indicator should be shown.
    /// </summary>
    public class MissionDeliveryAssistance : MonoBehaviour
    {
        #region Private properties

        private RepairObject[] _repairObjects = default;

        #endregion

        #region Unity events

        private void Awake()
        {
            _repairObjects = FindObjectsOfType<RepairObject>();

            if (_repairObjects != null) return;

            _repairObjects = new RepairObject[0];

            Debug.LogError("<color=orange>MissionDeliveryAssistance</color>::Awake() -> ERROR: no repair object was found!");
        }

        #endregion

        #region Public methods

        public (Transform target, Vector3 targetOffset, bool found) Process(int repairObjectId)
        {
            Vector3 offset = Vector3.zero;
            offset.y = 2;

            var (target, found) = GetRepairObject(repairObjectId);

            return (target, offset, found);
        }

        #endregion

        #region Private methods

        private (Transform target, bool found) GetRepairObject(int id)
        {
            for (int i = 0; i < _repairObjects.Length; i++)
            {
                var area = _repairObjects[i];

                if (area.Id == id) return (area.transform, true);
            }

            return (null, false);
        }

        #endregion
    }
}