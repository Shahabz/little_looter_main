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
		public System.Action<bool> OnStatusChanged;

		private void OnBecameVisible()
		{
			OnStatusChanged?.Invoke(true);
		}

		private void OnBecameInvisible()
		{
			OnStatusChanged?.Invoke(false);
		}
	}
}