/*
 * Date: October 1st, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
	/// <summary>
	/// Represents the behavior of repairable objects.
	/// When player interacts with it, based on its status, it can be repaired based on elements needed and repairing speed.
	/// </summary>
    public class RepairObject : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _indicator = default;
		[SerializeField] private RepairObjectData _data = default;

		#endregion

		#region Private properties

		private float _progress = 0;
		private float _progressGoal = 10;
		private float _speedRepairing = 0;
		private const float _step = 0.1f;
		private bool _isProcessing = false;
		private bool _wasCompleted = false;
		private System.Action _onComplete = default;

		#endregion

		#region Public properties

		public int Id => _data.Id;
		public bool IsRepairing => _isProcessing;
		public bool WasCompleted => _wasCompleted;

		public RepairObjectData Data => _data;

		#endregion

		#region Public methods

		public void ShowIndicator()
		{
			_indicator.SetActive(true);

			UI_GameplayEvents.OnShowRepairPanel?.Invoke(_data);
		}

		public void HideIndicator()
		{
            _indicator.SetActive(false);

			UI_GameplayEvents.OnHideRepairPanel?.Invoke();
        }

		#endregion
	}
}