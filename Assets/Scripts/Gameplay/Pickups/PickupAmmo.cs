
using UnityEngine;

namespace LittleLooters.Gameplay
{
	public class PickupAmmo : MonoBehaviour
	{
		[SerializeField] private GameObject _art = default;
		[SerializeField] private int _amount = 0;

		private Collider _collider = default;

		public int Amount => _amount;

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