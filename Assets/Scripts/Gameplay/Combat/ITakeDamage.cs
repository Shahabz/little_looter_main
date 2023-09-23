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
		event Action<float> OnTakeDamage;
        event Action OnDead;

		#endregion

		#region Public properties

		public bool IsDead { get; }
        public float Health { get; }
        public float MaxHealth { get; }

		#endregion

		#region Public methods

		void Init(float initialHp, float maxHp);

        void TakeDamage(float damage);

		#endregion
	}
}