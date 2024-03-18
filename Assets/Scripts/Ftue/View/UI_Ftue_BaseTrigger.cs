/*
 * Date: March 18th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Ftue.View
{
	public class UI_Ftue_BaseTrigger : MonoBehaviour
	{
		private const string _playerTag = "Player";
		
		public System.Action<bool> OnChanged;

		private void OnTriggerEnter(Collider collider)
		{
			if (!collider.CompareTag(_playerTag)) return;

			OnChanged?.Invoke(true);
		}

		private void OnTriggerExit(Collider collider)
		{
			if (!collider.CompareTag(_playerTag)) return;

			OnChanged?.Invoke(false);
		}
	}
}