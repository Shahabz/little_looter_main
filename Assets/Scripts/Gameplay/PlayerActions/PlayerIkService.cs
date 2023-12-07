/*
 * Date: December 7th, 2023
 * Author: Peche
 */

using DG.Tweening;
using LittleLooters.Gameplay.Combat;
using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace LittleLooters.Gameplay
{
    public class PlayerIkService : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private Transform _rightHandGrip = default;
        [SerializeField] private Transform _leftHandGrip = default;
        [SerializeField] private RigBuilder _rigBuilder = default;
        [SerializeField] private WeaponController _weaponController = default;

        [Header("Aiming IK configuration")]
        [SerializeField] private Vector3 _rightHandAimPosition = default;
        [SerializeField] private Vector3 _rightHandAimRotation = default;
        [SerializeField] private Vector3 _leftHandAimPosition = default;
        [SerializeField] private Vector3 _leftHandAimRotation = default;

        [Header("Idle IK configuration")]
        [SerializeField] private Vector3 _rightHandIdlePosition = default;
        [SerializeField] private Vector3 _rightHandIdleRotation = default;
        [SerializeField] private Vector3 _leftHandIdlePosition = default;
        [SerializeField] private Vector3 _leftHandIdleRotation = default;

        [Header("Firing IK configuration")]
        [SerializeField] private float _animDuration = default;
        [SerializeField] private Vector3 _animPositionGoal = default;
        [SerializeField] private bool _canAnimateFiring = true;

        #endregion

        #region Private properties

        private bool _isAiming = false;
        private bool _isAnimatingFiring = false;

		#endregion

		// left pos: -0.079, 0.012, 0.169
		// left rot: 251.386, -56.12399, 283.268

		// right pos: -0.006, -0.1, -0.158
		// right rot: 61.695, 93.576, -99.036

		#region Unity events

		private void Awake()
		{
            _weaponController.OnStartReloading += HandleOnStartReloading;
            _weaponController.OnStopReloading += HandleOnStopReloading;
            _weaponController.OnStartFiring += HandleOnStartFiring;
            _weaponController.OnCompleteFiring += HandleOnStopFiring;

            PlayerAimingAssistance.OnStartAiming += HandleOnStartAiming;
            PlayerAimingAssistance.OnStopAiming += HandleOnStopAiming;
        }

		private void Start()
		{
            SetupIdleConfiguration();
        }

		private void OnDestroy()
        {
            _weaponController.OnStartReloading -= HandleOnStartReloading;
            _weaponController.OnStopReloading -= HandleOnStopReloading;
            _weaponController.OnStartFiring -= HandleOnStartFiring;
            _weaponController.OnCompleteFiring -= HandleOnStopFiring;

            PlayerAimingAssistance.OnStartAiming -= HandleOnStartAiming;
            PlayerAimingAssistance.OnStopAiming -= HandleOnStopAiming;
        }

		#endregion

		#region Private methods

		private void HandleOnStartReloading(float reloadingTime)
		{
            _rigBuilder.enabled = false;
		}

        private void HandleOnStopReloading()
        {
            _rigBuilder.enabled = true;
        }

        private void HandleOnStopAiming()
        {
            if (!_isAiming) return;

            _isAiming = false;

            SetupIdleConfiguration();
        }

        private void HandleOnStartAiming()
        {
            if (_isAiming) return;

            _isAiming = true;

            SetupAimingConfiguration();
        }

        private void SetupIdleConfiguration()
        {
            // Right hand
            _rightHandGrip.localPosition = _rightHandIdlePosition;
            _rightHandGrip.localRotation = Quaternion.Euler(_rightHandIdleRotation);

            // Left hand
            _leftHandGrip.localPosition = _leftHandIdlePosition;
            _leftHandGrip.localRotation = Quaternion.Euler(_leftHandIdleRotation);
        }

        private void SetupAimingConfiguration()
        {
            // Right hand
            _rightHandGrip.localPosition = _rightHandAimPosition;
            _rightHandGrip.localRotation = Quaternion.Euler(_rightHandAimRotation);

            // Left hand
            _leftHandGrip.localPosition = _leftHandAimPosition;
            _leftHandGrip.localRotation = Quaternion.Euler(_leftHandAimRotation);
        }

        private void HandleOnStopFiring()
        {
            if (_isAiming) return;

            SetupIdleConfiguration();
        }

        private void HandleOnStartFiring(float fireRate)
        {
            ApplyFiringAnimation();

            if (_isAiming) return;

            SetupAimingConfiguration();
        }

        private void ApplyFiringAnimation()
		{
            if (!_canAnimateFiring) return;

            if (_isAnimatingFiring) return;

            _isAnimatingFiring = true;

            _rightHandGrip.transform.DOLocalMove(_animPositionGoal, _animDuration).OnComplete(OnHandlleFiringAnimationCompletion);
		}

		private void OnHandlleFiringAnimationCompletion()
		{
            var goal = (_isAiming) ? _rightHandAimPosition : _rightHandIdlePosition;

            _rightHandGrip.transform.DOLocalMove(goal, _animDuration).OnComplete( () => _isAnimatingFiring = false );
        }

		#endregion
	}
}