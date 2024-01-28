/*
 * Date: September 30th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
	/// <summary>
	/// Represents the player's equipable weapons
	/// </summary>
	public class UI_WeaponSelectionPanel : MonoBehaviour
	{
		[SerializeField] private WeaponController _weaponController = default;
		[SerializeField] private GameObject _panel = default;
		[SerializeField] private UI_WeaponSelectionSlot[] _slots = default;

		private void Start()
		{
			Init();
		}

		private void OnDestroy()
		{
			Teardown();
		}

		private void ShowPanel()
		{
			_panel.SetActive(true);
		}

		private void HidePanel()
		{
			_panel.SetActive(false);
		}

		private void Init()
		{
			LevelEnemies.OnStartDetection += HandleStartEnemiesDetection;
			LevelEnemies.OnStopDetection += HandleStopEnemiesDetection;

			InitSlots();

			HidePanel();
		}

		private void Teardown()
		{
			LevelEnemies.OnStartDetection -= HandleStartEnemiesDetection;
			LevelEnemies.OnStopDetection -= HandleStopEnemiesDetection;

			TeardownSlots();
		}

		private void HandleStopEnemiesDetection()
		{
			HidePanel();
		}

		private void HandleStartEnemiesDetection()
		{
			ShowPanel();
		}

		private void InitSlots()
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				var slot = _slots[i];

				slot.Init(i, i == 0, SlotSelection, _weaponController);
			}
		}

		private void TeardownSlots()
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				var slot = _slots[i];

				slot.Teardown(_weaponController);
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