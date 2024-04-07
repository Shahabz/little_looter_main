/*
 * Date: April 7th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class TriggerPlayerProximityObject : MonoBehaviour
    {
		public System.Action<GameObject, bool> OnStatusChanged;

		private const string _playerTag = "Player";

		private void OnTriggerEnter(Collider other)
		{
			if (!enabled) return;

			if (!other.CompareTag(_playerTag)) return;

			OnStatusChanged?.Invoke(gameObject, true);
		}

		private void OnTriggerExit(Collider other)
		{
			if (!enabled) return;

			if (!other.CompareTag(_playerTag)) return;

			OnStatusChanged?.Invoke(gameObject, false);
		}
	}
}