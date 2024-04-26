/*
 * Date: April 25th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class PlayerStaminaService : MonoBehaviour
    {
		#region Events

		public static System.Action<int> OnGrantPoints;
		public static System.Action<int> OnConsumePoints;

		#endregion

		#region Inspector

		[SerializeField] private int _points = 4;
		[SerializeField] private float _pointRecoveryTime = 3;

		#endregion

		#region Private properties

		private int _currentPoints = 0;
		private float _remainingTime = 0;

		#endregion

		#region Unity events

		private void Start()
		{
			GrantPoints(_points);

			_remainingTime = 0;
		}

		private void Update()
		{
			if (_remainingTime == 0) return;

			Refill(Time.deltaTime);
		}

		#endregion

		#region Public methods

		public bool TryConsumePoints(int amount)
		{
			if (amount > _currentPoints) return false;

			ConsumePoints(amount);

			if (_remainingTime > 0) return true;

			_remainingTime = _pointRecoveryTime;

			return true;
		}

		#endregion

		#region Private methods

		private void Refill(float deltaTime)
		{
			_remainingTime -= deltaTime;

			if (_remainingTime > 0) return;

			GrantPoints(1);

			if (_currentPoints == _points)
			{
				_remainingTime = 0;
				return;
			}

			_remainingTime = _pointRecoveryTime;
		}

		private void GrantPoints(int amount)
		{
			_currentPoints += amount;

			OnGrantPoints?.Invoke(_currentPoints);
		}

		private void ConsumePoints(int amount)
		{
			_currentPoints -= amount;

			OnConsumePoints?.Invoke(_currentPoints);
		}

		#endregion
	}
}