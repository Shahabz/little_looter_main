
using LittleLooters.Gameplay.Combat;
using UnityEngine;

namespace LittleLooters.Gameplay
{
	public class PlayerPickupsService : MonoBehaviour
	{
		[SerializeField] private WeaponController _weaponController = default;

		private const string _tag = "Pickup";

		private void OnTriggerEnter(Collider other)
		{
			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<PickupAmmo>(out var pickup)) return;

			pickup.Collect();

			ApplyPickup(pickup);
		}

		private void ApplyPickup(PickupAmmo pickup)
		{
			_weaponController.PickupAmmo(pickup.Amount);
		}
	}
}