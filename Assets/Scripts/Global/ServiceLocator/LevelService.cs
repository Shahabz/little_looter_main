/*
 * Date: March 9th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Global.ServiceLocator
{

    public class LevelService : MonoBehaviour, IGameService
    {
        [SerializeField] private Transform _toolUpgradeArea = default;

        public Transform ToolUpgradeArea => _toolUpgradeArea;
    }
}