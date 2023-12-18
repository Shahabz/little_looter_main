/*
 * Date: December 16th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using System;

namespace LittleLooters.Gameplay
{
    public class PlayerCraftingEvents
    {
        /// <summary>
        /// Invoked when player is near to a crafting area
        /// </summary>
        public static Action<CraftingConfigurationData> OnStartAreaInteraction;

        /// <summary>
        /// Invoked when player is going out from a crafting detection area
        /// </summary>
        public static Action OnStopAreaInteraction;
    }
}