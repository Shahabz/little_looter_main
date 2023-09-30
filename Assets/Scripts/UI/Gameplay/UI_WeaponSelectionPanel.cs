/*
 * Date: September 30th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
	/// <summary>
	/// Represents the player's equipable weapons
	/// </summary>
	public class UI_WeaponSelectionPanel : MonoBehaviour
	{
		[SerializeField] private UI_WeaponSelectionSlot[] _slots = default;

		private void Start()
		{
			Init();
		}

		private void OnDestroy()
		{
			Teardown();
		}

		private void Init()
		{
			InitSlots();
		}

		private void Teardown()
		{
			TeardownSlots();
		}

		private void InitSlots()
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				var slot = _slots[i];

				slot.Init(i, i == 0, SlotSelection);
			}
		}

		private void TeardownSlots()
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				var slot = _slots[i];

				slot.Teardown();
			}
		}

		private void SlotSelection(int id)
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				var slot = _slots[i];

				if (slot.Id == id)
				{
					slot.MarkAsSelected();
					continue;
				}

				slot.MarkAsNonSelected();
			}

			UI_GameplayEvents.OnWeaponSelection?.Invoke(id);
		}
	}
}