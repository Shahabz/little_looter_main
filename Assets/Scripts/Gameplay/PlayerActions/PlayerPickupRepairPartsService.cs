/*
 * Date: October 1st, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
	public class PlayerPickupRepairPartsService : MonoBehaviour
	{
		[SerializeField] private PlayerRepairService _service = default;

		private const string _tag = "RepairPart";

		private void OnTriggerEnter(Collider other)
		{
			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<RepairPart>(out var repairPart)) return;

			ApplyPickup(repairPart);
		}

		private void ApplyPickup(RepairPart repairPart)
		{
			var canPickup = _service.CanPickup();

			if (!canPickup) return;

			repairPart.Pickup();

			_service.PickupPart(repairPart.Data);
		}
	}
}