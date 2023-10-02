/*
 * Date: October 1st, 2023
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_RepairPartsPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _active = default;
		[SerializeField] private Image _icon = default;

		#endregion

		#region Unity events

		private void Start()
		{
			UI_GameplayEvents.OnPickupedRepairPart += Pickup;
			UI_GameplayEvents.OnConsumedRepairPart += Drop;
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnPickupedRepairPart -= Pickup;
			UI_GameplayEvents.OnConsumedRepairPart -= Drop;
		}

		#endregion

		#region Private methods

		private void Show(RepairPartData data)
		{
			_icon.sprite = data.Icon;

			_active.SetActive(true);
		}

		private void Hide()
		{
			_active.SetActive(false);
		}

		private void Pickup(RepairPartData data)
		{
			Show(data);
		}

		private void Drop(RepairPartData data)
		{
			Hide();
		}

		#endregion
	}
}