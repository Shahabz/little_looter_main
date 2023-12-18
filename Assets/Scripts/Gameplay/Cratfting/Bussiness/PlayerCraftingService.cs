/*
 * Date: December 14th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class PlayerCraftingService : MonoBehaviour
    {
		#region Private properties

		private const string _tag = "Crafting";

		#endregion

		#region Unity events

		private void OnTriggerEnter(Collider other)
		{
			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<CraftingArea>(out var craftingArea)) return;

			StartInteraction(craftingArea);
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<CraftingArea>(out var craftingArea)) return;

			StopInteraction(craftingArea);
		}

		#endregion

		#region Private methods

		private void StartInteraction(CraftingArea area)
		{
			area.StartInteraction();
		}

		private void StopInteraction(CraftingArea area)
		{
			area.StopInteraction();
		}

		#endregion
	}
}