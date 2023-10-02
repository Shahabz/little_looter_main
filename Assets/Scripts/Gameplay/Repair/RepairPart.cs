/*
 * Date: October 1st, 2023
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay
{
    public class RepairPart : MonoBehaviour
    {
        [SerializeField] private RepairPartData _data = default;
        [SerializeField] private Image _icon = default;
        [SerializeField] private GameObject _art = default;
        [SerializeField] private Collider _collider = default;

        public RepairPartData Data => _data;

		private void Awake()
		{
            _icon.sprite = _data.Icon;
		}

		public void Pickup()
        {
            _art.SetActive(false);

            _collider.enabled = false;
        }
    }
}