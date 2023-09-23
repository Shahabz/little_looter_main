/*
 * Date: September 16th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay.Combat
{
    /// <summary>
    /// In charge of target detection based on entity forward direction, radius and angle to define field of view.
    /// </summary>
    public class EnemyFieldOfViewService
    {
		#region Events

		public System.Action OnStartDetection;
		public System.Action OnStopDetection;

        #endregion

        #region Private properties

        private Transform target = default;
        private Transform transform = default;
        private bool targetDetected = false;
        private EnemyBehaviorData behaviorData = default;

        #endregion

        #region Public properties

        public bool TargetDetected => this.targetDetected;

		#endregion

		#region Public methods

		public void Init(Transform localTransform, Transform target, EnemyBehaviorData behaviorData)
		{
            this.transform = localTransform;
            this.target = target;
            this.behaviorData = behaviorData;
		}

        public void Tick()
		{
            CheckTarget();
		}

        #endregion

        #region Private methods

        /// <summary>
        /// For each entity alive, check which one is the nearest inside the angle of field of view,
        /// inside the area of radius detection
        /// </summary>
        private void CheckTarget()
        {
            Vector3 currentPosition = this.transform.position;

            Vector3 directionToTarget = this.target.position - currentPosition;

            // Checks if the distance is inside the area radius detection
            var insideRange = CheckInsideRange(directionToTarget);

            if (!insideRange)
			{
                StopDetection();

                return;
			}

            // Check dot product to detect if the target entity is inside of field of view
            var insideFOV = GetAngle(directionToTarget.normalized);

            if (!insideFOV)
			{
                StopDetection();

                return;
			}

            StartDetection();
        }

        private bool CheckInsideRange(Vector3 directionToTarget)
		{
            float dSqrToTarget = directionToTarget.magnitude;

            var insideRange = dSqrToTarget <= this.behaviorData.RadiusDetection;

            return insideRange;
        }

        private bool GetAngle(Vector3 direction)
        {
            var angleToTarget = Mathf.Abs(Vector3.Angle(direction, this.transform.forward));

            if (angleToTarget > this.behaviorData.AngleFieldOfView/2) return false;

            return true;
        }

        private void StopDetection()
		{
            if (!this.targetDetected) return;

            this.targetDetected = false;

            OnStopDetection?.Invoke();
        }

        private void StartDetection()
		{
            if (this.targetDetected) return;

            this.targetDetected = true;

            OnStartDetection?.Invoke();
		}

        #endregion
    }
}