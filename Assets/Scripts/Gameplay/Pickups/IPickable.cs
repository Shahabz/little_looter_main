/*
 * Date: December 28th, 2023
 * Author: Peche
 */

namespace LittleLooters.Gameplay
{
    public interface IPickable
    {
        int Id { get; }
        PickableType Type { get; }
        int Amount { get; }

        void Collect();
    }
}