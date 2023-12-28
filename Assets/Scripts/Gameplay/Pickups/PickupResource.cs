/*
 * Date: December 28th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using UnityEngine;

namespace LittleLooters.Gameplay
{
	public class PickupResource : MonoBehaviour, IPickable
	{
		[SerializeField] private ResourceData _data = default;
		[SerializeField] private GameObject _art = default;
		[SerializeField] private int _amount = 0;

		private Collider _collider = default;

		public int Id => _data.Id;
		public int Amount => _amount;
		public PickableType Type => PickableType.RESOURCE;

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