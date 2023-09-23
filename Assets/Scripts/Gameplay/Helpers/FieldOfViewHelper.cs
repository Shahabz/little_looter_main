/*
 * Date: September 2nd, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    /// <summary>
    /// Helper used to visualize the detection cone from an entity
    /// </summary>
    public class FieldOfViewHelper : MonoBehaviour
    {
		#region Private properties

		private bool _initialized = false;
        private EnemyBehaviorData _behaviorData = default;
        private Transform _transform = default;
        private Vector3 _forward = default;

		#endregion

		#region Public methods

		public void Init(EnemyBehaviorData data)
        {
            _transform = transform;

            _behaviorData = data;

            _initialized = true;
        }

        public void Tick(Vector3 forward)
        {
            _forward = forward;
        }

		#endregion

		#region Unity events

#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
        {
            if (!_initialized) return;

            Color c = new Color(0.8f, 0, 0, 0.4f);
            UnityEditor.Handles.color = c;

            var forward = (_forward == Vector3.zero) ? _transform.forward : _forward;

            Vector3 rotatedForward = Quaternion.Euler(0, -_behaviorData.AngleFieldOfView * 0.5f, 0) * forward;

            UnityEditor.Handles.DrawSolidArc(_transform.position, Vector3.up, rotatedForward, _behaviorData.AngleFieldOfView, _behaviorData.RadiusDetection);
        }
#endif

		#endregion
	}
}
