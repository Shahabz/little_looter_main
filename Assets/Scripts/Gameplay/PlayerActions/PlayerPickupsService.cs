
using LittleLooters.Gameplay.Combat;
using LittleLooters.Global.ServiceLocator;
using UnityEngine;

namespace LittleLooters.Gameplay
{
	public class PlayerPickupsService : MonoBehaviour
	{
		[SerializeField] private PlayerEntryPoint _playerEntryPoint = default;
		[SerializeField] private WeaponController _weaponController = default;

		private const string _tag = "Pickup";

		private void OnTriggerEnter(Collider other)
		{
			if (!other.tag.Equals(_tag)) return;

			if (!other.gameObject.TryGetComponent<IPickable>(out var pickup)) return;

			pickup.Collect();

			Pickup(pickup);
		}

		private void Pickup(IPickable pickup)
		{
			if (pickup.Type == PickableType.AMMO)
			{
				_weaponController.PickupAmmo(pickup.Amount);
				return;
			}

			if (pickup.Type == PickableType.RESOURCE)
			{
				var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();
				progressDataService.Resources_GrantByPickup(pickup.Id, pickup.Amount);

				return;
			}
		}
	}
}