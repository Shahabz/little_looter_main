/*
 * Date: September 28th, 2023
 * Author: Peche
 */

using System.Collections.Generic;
using UnityEngine;

namespace LittleLooters.Gameplay.Combat
{
    public class PlayerAimingAssistance
    {
		#region Events

		public static System.Action OnStartAiming;
        public static System.Action OnStopAiming;

		#endregion

		#region Private properties

		private Transform localTransform = default;
        private float angleThreshold = default;
        private float radiusDetection = default;
        private LevelEnemies levelEnemies = default;
        private EnemyController[] results = default;
        private bool enabled = false;
        private bool _targetDetected = false;
        private Transform _target = default;

		#endregion

		#region Public properties

		public bool TargetDetected => _targetDetected;
        public Transform Target => _target;

		#endregion

		#region Public methods

		public void Init(Transform transform, float angle, float radius, LevelEnemies levelEnemies)
		{
            this.localTransform = transform;
            this.angleThreshold = angle;
            this.radiusDetection = radius;
            this.levelEnemies = levelEnemies;

            this.results = new EnemyController[0];

            this.enabled = true;
		}

        public bool Process(Vector3 forward)
		{
            if (!this.enabled) return false;

            var response = CalculateTargetInRange(forward);

            if (!response.inRange)
            {
                StopProcessing();

                return false;
            }

            levelEnemies.StartDetection(response.target);

            _targetDetected = true;

            _target = response.target.transform;

            OnStartAiming?.Invoke();

            return true;
		}

        public void StopProcessing()
		{
            if (!this.enabled) return;

            _targetDetected = false;
            _target = null;

            OnStopAiming?.Invoke();

            levelEnemies.StopDetection();
        }

        public void RotateToTarget()
        {
            if (!_targetDetected) return;

            var target = _target.position;

            target.y = this.localTransform.position.y;

            this.localTransform.LookAt(target);
        }

        #endregion

        #region Private methods

        private (bool inRange, EnemyController target) CalculateTargetInRange(Vector3 forward)
		{
            var inRange = false;
            EnemyController targetEnemy = null;
            EnemyController nearestTarget = null;
            var minDistance = float.MaxValue;

            // Calculate possible targets inside a circle
            var targetsAround = GetTargetsAround();

            if (!targetsAround) return (false, null);

            var minAngle = float.MaxValue;

            var playerPosition = this.localTransform.position;

            // For each possible target check if there is at least one in the cone vision
            for (int i = 0; i < this.results.Length; i++)
			{
                var possibleTarget = results[i];

                var directionToTarget = possibleTarget.transform.position - playerPosition;

                var response = GetAngle(directionToTarget, forward);

                if (!response.found) continue;

                inRange = true;

                //if (response.angle > minAngle) continue;

                targetEnemy = possibleTarget;

                //minAngle = response.angle;

                var distance = directionToTarget.sqrMagnitude;

                if (distance >= minDistance) continue;

                nearestTarget = targetEnemy;

                minDistance = distance;
			}

            return (inRange, nearestTarget);
		}

        private (bool found, float angle) GetAngle(Vector3 direction, Vector3 forward)
        {
            //var angleToTarget = Mathf.Abs(Vector3.Angle(direction, this.localTransform.forward));
            var angleToTarget = Mathf.Abs(Vector3.Angle(direction, forward));

            if (angleToTarget > this.angleThreshold / 2) return (false, 0);

            return (true, angleToTarget);
        }

        private bool GetTargetsAround()
		{
            var currentPosition = this.localTransform.position;

            var found = false;

            var enemies = this.levelEnemies.Entities;

            var entities = new List<EnemyController>();

			for (int i = 0; i < enemies.Length; i++)
			{
                var enemy = enemies[i];

                if (enemy.IsDead) continue;

                var directionToTarget = enemy.transform.position - currentPosition;

                float dSqrToTarget = directionToTarget.magnitude;

                var insideRange = dSqrToTarget <= this.radiusDetection;

                if (!insideRange) continue;

                entities.Add(enemy);
                found = true;
            }

            results = entities.ToArray();

            return found;
		}

        #endregion
    }
}