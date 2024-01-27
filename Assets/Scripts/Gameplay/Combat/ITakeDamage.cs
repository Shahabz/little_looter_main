/*
 * Date: September 18th, 2023
 * Author: Peche
 */

using System;

namespace LittleLooters.Gameplay.Combat
{
    public interface ITakeDamage
    {
		#region Events

		event Action OnInitialized;
		event Action<int> OnTakeDamage;
        event Action OnDead;

		#endregion

		#region Public properties

		public bool IsDead { get; }
        public int Health { get; }
        public int MaxHealth { get; }

		#endregion

		#region Public methods

		void Init(int initialHp, int maxHp);

        void TakeDamage(int damage);

		#endregion
	}
}