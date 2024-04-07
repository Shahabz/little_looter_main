/*
 * Date: March 31th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    /// <summary>
    /// Class in charge of controlling the state of an indicator to inform about its position outside screen
    /// </summary>
    public class UI_TargetIndicator : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private GameObject _content = default;
		public RectTransform _indicator = default;
        public RectTransform _indicatorRotator = default;
        public Transform _player = default;
        public Transform _target = default;
        [SerializeField] private Camera _camera = default;
        [SerializeField] private RectTransform _canvasRect = default;

        [Header("Horizontal offsets")]
        [SerializeField] private float _offsetL = 100f;
        [SerializeField] private float _offsetR = 100f;

        [Header("Vertical offsets")]
        [SerializeField] private float _offsetTop = 50f;
        [SerializeField] private float _offsetBottom = 50f;

        [Header("Hiden distance properties")]
        [SerializeField] private bool _checkHidenDistance = true;
        [SerializeField] private float _distanceToHide = 3f;

		#endregion

		#region Private properties

		private Vector3 _eulerAngles = Vector3.zero;
        private bool _isHiden = false;
        private bool _isActive = false;

		#endregion

		#region Unity events

		private void Start()
		{
            Init();
		}

		/// <summary>
		/// CAN BE REPLACED FOR INVOKE REPEAT METHOD!
		/// </summary>
		private void Update()
		{
            if (!_isActive) return;

            CheckVisibleStatus();

            if (_isHiden) return;

            RefreshIndicatorAngle();
            RefreshIndicatorPosition();
        }

		private void OnDestroy()
		{
            Teardown();
		}

		#endregion

		#region Public methods

		protected virtual void Init() { }

        protected virtual void Teardown() { }

        public void Activate()
		{
            _isActive = true;

            if (_content.activeSelf) return;

            _content.SetActive(true);
		}

        public void Deactivate()
		{
            _isActive = false;

            if (!_content.activeSelf) return;

            _content.SetActive(false);
		}

		#endregion

		#region Private methods

		private void RefreshIndicatorAngle()
		{
            Vector3 playerPoint = _camera.WorldToScreenPoint(_player.position);
            Vector3 targetPoint = _camera.WorldToScreenPoint(_target.position);

            var dir = targetPoint - playerPoint;

            var angle = GetAngleFromVectorFloat(dir);

            _eulerAngles.z = angle - 90;

            _indicatorRotator.localEulerAngles = _eulerAngles;
        }

        private void RefreshIndicatorPosition()
		{
            var targetPosition = _target.position;

            Vector3 targetPositionScreenPoint = _camera.WorldToScreenPoint(targetPosition);

            var screenWidth = Screen.width; //_canvasRect.rect.width;
            var screenHeight = Screen.height;   //_canvasRect.rect.height;

            _indicator.position = ClampMarkerIndicator(targetPositionScreenPoint, screenWidth, screenHeight);
        }

        private float GetAngleFromVectorFloat(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        private void ShowContent()
        {
            if (_content.activeSelf) return;

            _content.SetActive(true);
        }

        private void HideContent()
        {
            if (!_content.activeSelf) return;

            _content.SetActive(false);
        }

        private void CheckVisibleStatus()
		{
            if (!_checkHidenDistance)
			{
                _isHiden = false;
                return;
			}

            Vector3 playerPoint = _camera.WorldToScreenPoint(_player.position);
            Vector3 targetPoint = _camera.WorldToScreenPoint(_target.position);

            var dir = targetPoint - playerPoint;

            var distance = dir.magnitude;

            _isHiden = distance <= _distanceToHide;

            if (_canDebug) DebugDistanceToHide(dir);

            if (_isHiden)
			{
                HideContent();
                return;
			}

            ShowContent();
        }

        private Vector3 ClampMarkerIndicator(Vector3 pos, float screenWidth, float screenHeight)
		{
#if UNITY_EDITOR
            return ClampMarkerIndicator_Editor(pos, screenWidth, screenHeight);
#else
            return ClampMarkerIndicator_Mobile(pos, screenWidth, screenHeight);
#endif
		}

        private Vector3 ClampMarkerIndicator_Editor(Vector3 pos, float screenWidth, float screenHeight)
		{
            // Check horizontal
            var limitL = 0 + _offsetL;
            var limitR = screenWidth - _offsetR;

            if (pos.x < limitL)
            {
                pos.x = limitL;
            }
            else if (pos.x > limitR)
            {
                pos.x = limitR;
            }

            // Check vertical
            var limitTop = screenHeight - _offsetTop;
            var limitBottom = 0 + _offsetBottom;

            if (pos.y > limitTop)
            {
                pos.y = limitTop;
            }
            else if (pos.y < limitBottom)
            {
                pos.y = limitBottom;
            }

            //Debug.LogError($"pos [{pos.x}, {pos.y}], screen [{screenWidth}, {screenHeight}], limit Horizontal: [{limitL}, {limitR}], limit vertical: [{limitTop}, {limitBottom}]");

            pos.z = 0;

            return pos;
        }

        private Vector3 ClampMarkerIndicator_Mobile(Vector3 pos, float screenWidth, float screenHeight)
        {
            // Check horizontal
            var limitL = 0 + _offsetL + 50;
            var limitR = (screenWidth) - (_offsetR + 50);

            if (pos.x < limitL)
            {
                pos.x = limitL;
            }
            else if (pos.x > limitR)
            {
                pos.x = limitR;
            }

            // Check vertical
            var limitTop = screenHeight - (_offsetTop + 50);
            var limitBottom = 0 + _offsetBottom + 50;

            if (pos.y > limitTop)
            {
                pos.y = limitTop;
            }
            else if (pos.y < limitBottom)
            {
                pos.y = limitBottom;
            }

            //Debug.LogError($"pos [{pos.x}, {pos.y}], screen [{screenWidth}, {screenHeight}], limit Horizontal: [{limitL}, {limitR}], limit vertical: [{limitTop}, {limitBottom}]");

            pos.z = 0;

            return pos;
        }

#endregion

#region Debug

        private bool _canDebug = false;

        private void DebugDistanceToHide(Vector3 dir)
		{
            Debug.LogError($"distance magnitude: <color=yellow>{dir.magnitude}</color>, distance magnitude sqr: <color=yellow>{dir.sqrMagnitude}</color>");
        }

#endregion
	}
}