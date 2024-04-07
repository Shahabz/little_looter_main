/*
 * Date: April 6th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
	/// <summary>
	/// Informs when the object is visible/hidden by camera.
	/// It requires an object with renderer component.
	/// </summary>
	[RequireComponent(typeof(MeshRenderer))]
    public class VisibleObject : MonoBehaviour
    {
		public System.Action<GameObject, bool> OnStatusChanged;

		private bool _isEnabled = false;

		private void OnEnable()
		{
			_isEnabled = true;
		}

		private void OnDisable()
		{
			_isEnabled = false;
		}

		private void OnBecameVisible()
		{
			if (!_isEnabled) return;

			OnStatusChanged?.Invoke(gameObject, true);
		}

		private void OnBecameInvisible()
		{
			if (!_isEnabled) return;

			OnStatusChanged?.Invoke(gameObject, false);
		}
	}
}