
using UnityEngine;

namespace LittleLooters.Gameplay
{
	public class PickupAmmo : MonoBehaviour, IPickable
	{
		[SerializeField] private int _id = 0;
		[SerializeField] private GameObject _art = default;
		[SerializeField] private int _amount = 0;

		private Collider _collider = default;

		public int Id => _id;
		public int Amount => _amount;
		public PickableType Type => PickableType.AMMO;

		private void Awake()
		{
			_collider = GetComponent<Collider>();
		}

		public void Collect()
		{
			_art.SetActive(false);
			_collider.enabled = false;
		}
	}
}