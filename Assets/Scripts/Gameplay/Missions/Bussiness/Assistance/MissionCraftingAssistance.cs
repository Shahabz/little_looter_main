/*
 * Date: December 27th, 2023
 * Author: Peche
 */

using System.Collections.Generic;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    /// <summary>
	/// Logic related with crafting mission assistance.
	/// When it is triggered, it tries to detect the nearest crafting area related with the mission.
	/// If it is on screen, then the indicator should be shown, else, the offscreen indicator should be shown.
	/// </summary>
    public class MissionCraftingAssistance : MonoBehaviour
    {
		#region Private properties

		private CraftingArea[] _areas = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			_areas = FindObjectsOfType<CraftingArea>();

			if (_areas != null) return;

			_areas = new CraftingArea[0];

			Debug.LogError("<color=orange>MissionCraftingAssistance</color>::Awake() -> ERROR: no area was found!");
		}

		#endregion

		#region Public methods

		public (Transform target, Vector3 targetOffset, bool found) Process(int resourceId)
		{
			Vector3 offset = Vector3.zero;
			offset.y = 2;

			var (target, found) = GetCraftingArea(resourceId);

			return (target, offset, found);
		}

		#endregion

		#region Private methods

		private (Transform target, bool found) GetCraftingArea(int id)
		{
			for (int i = 0; i < _areas.Length; i++)
			{
				var area = _areas[i];

				if (area.Data.ResourceGenerated.Id == id) return (area.transform, true);
			}

			return (null, false);
		}

		#endregion
	}
}