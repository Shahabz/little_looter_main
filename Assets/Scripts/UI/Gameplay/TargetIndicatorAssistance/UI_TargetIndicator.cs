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
        [SerializeField] private bool _useAngle = default;
        [SerializeField] private bool _useSigned = default;
        [SerializeField] private float outOfSightOffest = 5;
        [SerializeField] private float borderSize = 100f;
        [SerializeField] private float borderSizeVertical = 100f;
        [SerializeField] private float offsetX = 100f;
        [SerializeField] private float offsetY = 100f;
        [SerializeField] private float offsetXLeft = 100f;
        [SerializeField] private float offsetYBottom = 100f;

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

            var screenWidth = _canvasRect.rect.width;
            var screenHeight = _canvasRect.rect.height;

            bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;

            //if (!isOffScreen)
			//{
            //    Hide();
            //    return;
			//}


            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
            if (cappedTargetScreenPosition.x >= screenWidth - borderSize) cappedTargetScreenPosition.x = screenWidth - borderSize;
            if (cappedTargetScreenPosition.y <= borderSizeVertical) cappedTargetScreenPosition.y = borderSizeVertical;
            if (cappedTargetScreenPosition.y >= screenHeight - borderSizeVertical) cappedTargetScreenPosition.y = screenHeight - borderSizeVertical;

            var clampedPos = ClampPosition(cappedTargetScreenPosition, screenWidth, screenHeight);
            _indicator.position = clampedPos;

            //Show();
        }

        private float GetAngleFromVectorFloat(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        private Vector3 ClampPosition(Vector3 pos, float screenWidth, float screenHeight)
		{
            pos.x = Mathf.Clamp(pos.x, -screenWidth/2 - offsetXLeft, screenWidth/2 - offsetX);
            pos.y = Mathf.Clamp(pos.y, -screenHeight/2 - offsetYBottom, screenHeight/2 - offsetY);

            return pos;
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