/*
 * Date: December 14th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay
{
    public class CraftingArea : MonoBehaviour
    {
        [SerializeField] private CraftingConfigurationData _data = default;
		[SerializeField] private Image _uiSelection = default;

        public int Id => _data.Id;

		#region Public methods

        public void StartInteraction()
		{
			if (_canDebug) DebugStartDetection();

			PlayerCraftingEvents.OnStartAreaInteraction?.Invoke(_data);

			_uiSelection.enabled = true;
		}

		public void StopInteraction()
		{
			if (_canDebug) DebugStopDetection();

			PlayerCraftingEvents.OnStopAreaInteraction?.Invoke();

			_uiSelection.enabled = false;
		}

		#endregion

		#region Debug

		private bool _canDebug = false;

		private void DebugStartDetection()
		{
			Debug.LogError($"Area <color=yellow>{_data.Id}</color> was <color=green>detected</color>");
		}

		private void DebugStopDetection()
		{
			Debug.LogError($"Area <color=yellow>{_data.Id}</color> stop <color=red>detection</color>");
		}

		#endregion
	}
}