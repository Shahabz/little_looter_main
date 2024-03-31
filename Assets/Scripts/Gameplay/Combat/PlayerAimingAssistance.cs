/*
 * Date: September 28th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
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

        private LayerMask obstaclesLayerMask = default;
        private RaycastHit[] obstacleHits = default;
        private Transform localTransform = default;
        private float angleThreshold = default;
        private const float radiusDetection = 15f;
        private float weaponRadiusDetection = default;
        private LevelEnemies levelEnemies = default;
        private EnemyController[] results = default;
        private bool enabled = false;
        private bool _targetDetected = false;
        private bool _isTargetInsideRadius = false;
        private Transform _target = default;
        private float _rotationVelocity;
        private float _rotationSpeed = 0.10f;

        #endregion

        #region Public properties

        public bool TargetDetected => _targetDetected;
        public bool TargetInsideRadius => _isTargetInsideRadius;
        public Transform Target => _target;

		#endregion

		#region Public methods

		public void Init(Transform transform, float angle, float radius, LevelEnemies levelEnemies, LayerMask obstaclesMask)
		{
            this.localTransform = transform;
            this.angleThreshold = angle;
            this.weaponRadiusDetection = radius;
            this.levelEnemies = levelEnemies;
            this.obstaclesLayerMask = obstaclesMask;

            this.results = new EnemyController[0];

            this.obstacleHits = new RaycastHit[4];

            PlayerProgressEvents.OnWeaponChanged += HandlePlayerWeaponChanged;

            this.enabled = true;
		}

        public void Teardown()
		{
            PlayerProgressEvents.OnWeaponChanged -= HandlePlayerWeaponChanged;
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

            levelEnemies.StartDetection(response.target, response.targetInsideRadius);

            _targetDetected = true;

            _isTargetInsideRadius = response.targetInsideRadius;

            if (_isTargetInsideRadius)
            {
                _target = response.target.transform;
            }

            OnStartAiming?.Invoke();

            return true;
		}

        public void StopProcessing()
		{
            if (!this.enabled) return;

            _targetDetected = false;
            _isTargetInsideRadius = false;
            _target = null;

            OnStopAiming?.Invoke();

            levelEnemies.StopDetection();
        }

        public void RotateToTarget(bool instant)
        {
            if (!_isTargetInsideRadius) return;

            if (!_targetDetected) return;

            var target = _target.position;

            target.y = this.localTransform.position.y;

            // Instant rotation
            if (instant)
            {
                this.localTransform.LookAt(target);
                return;
            }

            // Smooth rotation
            var targetDirection = (target - this.localTransform.position).normalized;

            var targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

            float rotation = Mathf.SmoothDampAngle(this.localTransform.eulerAngles.y, targetRotation, ref _rotationVelocity, _rotationSpeed);

            // rotate to face target direction relative to camera position
            this.localTransform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        #endregion

        #region Private methods

        private void HandlePlayerWeaponChanged(AssaultWeaponData weaponData)
		{
            this.weaponRadiusDetection = weaponData.RadiusDetection;

            UI_GameplayEvents.OnWeaponRadiusChanged?.Invoke(this.weaponRadiusDetection);
		}

        private (bool inRange, EnemyController target, bool targetInsideRadius) CalculateTargetInRange(Vector3 forward)
		{
            var inRange = false;
            EnemyController targetEnemy = null;
            EnemyController nearestTarget = null;
            var minDistance = float.MaxValue;

            // Calculate possible targets inside a circle
            var targetsAround = GetTargetsAround();

            if (!targetsAround) return (false, null, false);

            var minAngle = float.MaxValue;

            var playerPosition = this.localTransform.position;
            var targetInsideRadius = false;

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

                var magnitude = directionToTarget.magnitude;

                // Check if target is inside player's weapon radius detection
                if (magnitude > this.weaponRadiusDetection) continue;

                nearestTarget = targetEnemy;

                minDistance = distance;

                targetInsideRadius = true;
			}


            return (inRange, nearestTarget, targetInsideRadius);
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

                // Check if target is inside the detection area
                var directionToTarget = enemy.transform.position - currentPosition;

                float dSqrToTarget = directionToTarget.magnitude;

                var insideRange = dSqrToTarget <= radiusDetection;

                if (!insideRange) continue;

                // Check if there is an obstacle between player and target
                var obstacleExist = CheckObstacleInBetween(currentPosition, directionToTarget);

                if (obstacleExist) continue;

                entities.Add(enemy);
                found = true;
            }

            results = entities.ToArray();

            return found;
		}

        /// <summary>
        /// Checks if there is an obstacle between target and player
        /// </summary>
        private bool CheckObstacleInBetween(Vector3 playerPosition, Vector3 targetDirection)
		{
            var result = Physics.RaycastNonAlloc(playerPosition, targetDirection, obstacleHits, targetDirection.magnitude, obstaclesLayerMask);

            //Debug.DrawRay(playerPosition, targetDirection, (result > 0) ? Color.red : Color.green);

            return result > 0;
		}

        #endregion
    }
}