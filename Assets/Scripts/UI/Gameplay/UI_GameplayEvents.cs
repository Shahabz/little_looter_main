/*
 * Date: September 30th, 2023
 * Author: Peche
 */

using System;

namespace LittleLooters.Gameplay
{
    public static class UI_GameplayEvents
    {
        /// <summary>
        /// Triggered when UI slot weapon is selected and player should swap its current equipped weapon
        /// </summary>
        public static Action<int> OnWeaponSelection;
    }
}