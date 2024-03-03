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

		private bool _firstTime = true;

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
			//LevelEnemies.OnStopDetection += HandleStopEnemiesDetection;

			InitSlots();

			HidePanel();
		}

		private void Teardown()
		{
			LevelEnemies.OnStartDetection -= HandleStartEnemiesDetection;
			//LevelEnemies.OnStopDetection -= HandleStopEnemiesDetection;

			TeardownSlots();
		}

		private void HandleStopEnemiesDetection()
		{
			HidePanel();
		}

		private void HandleStartEnemiesDetection()
		{
			if (!_firstTime) return;

			_firstTime = false;

			ShowPanel();
		}

		private void InitSlots()
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				var slot = _slots[i];

				var weaponInfo = _weaponController.GetWeaponInfo(i);
				var weaponMagazineSize = _weaponController.GetWeaponMagazineSize(weaponInfo.id);

				slot.Init(weaponInfo, i == 0, SlotSelection, weaponMagazineSize);
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

		private void SlotSelection(string id)
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				var slot = _slots[i];

				if (slot.Id.Equals(id))
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