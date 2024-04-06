/*
 * Date: October 22th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    /// <summary>
    /// Logic to make an object follow the camera position and/or rotation
    /// Link: https://forum.unity.com/threads/make-ui-or-image-following-facing-smoothly-to-camera-like-the-unity-vr-splashscreen.835957/
    /// </summary>
    public class UI_FollowCamera : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private Camera _cam = default;
        [SerializeField] private Transform _camera = default;
        [SerializeField] private Transform _targetPosition = default;
        [SerializeField] private bool _followRotation = true;
        [SerializeField] private bool _followPosition = false;
        [SerializeField] private bool _autoInitialization = false;

        #endregion

        #region Private properties

        private bool _initialized = false;
        private bool _isEnable = true;
        private Transform _transform = default;
        private float _cameraDistance = 3.0f;
        private float _smoothTime = 0.3f;
        private Vector3 _velocity = Vector3.zero;

        #endregion

        #region Public methods

        public void Init()
        {
            // TODO: improvement
            // _camera = LevelManager.Instance.Camera.transform;

            _transform = transform;

            _initialized = true;
        }

        public void SetEnable()
        {
            _isEnable = true;
        }

        public void SetDisable()
        {
            _isEnable = false;
        }

        public void RefreshTransform(Transform target)
		{
            _transform = target;
		}

        #endregion

        #region Unity events

        private void Start()
        {
            if (!_autoInitialization) return;

            Init();
        }

        private void LateUpdate()
        {
            if (!_initialized) return;

            if (!_isEnable) return;

            FollowCamera();
        }

        #endregion

        #region Private methods

        private void FollowCamera()
        {
            // Follow rotation
            if (_followRotation && !_followPosition)
            {
                FollowRotation();

                return;
            }

            // Follow position
            if (_followPosition) // && !_followRotation)
            {
                FollowPosition();

                return;
            }
        }

        private void FollowRotation()
        {
            _transform.LookAt(transform.position + _camera.rotation * Vector3.forward, _camera.rotation * Vector3.up);
        }

        private void FollowPosition()
        {
            var position = _cam.WorldToScreenPoint(_targetPosition.position);

            transform.position = position;
        }

        #endregion
    }
}