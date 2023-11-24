/*
 * Date: November 23th, 2023
 * Author: Peche
 */

using System;

namespace LittleLooters.Gameplay
{
    public class ExplorableObjectEvents
    {
        public static Action<ExplorableObjectType> OnEnter;
        public static Action<ExplorableObjectType> OnExit;
    }
}