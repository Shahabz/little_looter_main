/*
 * Date: September 16th, 2023
 * Author: Peche
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_CameraRotatorPanel : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Button _btnL = default;
        [SerializeField] private Button _btnR = default;
        [SerializeField] private Transform _cameraPivot = default;
		[SerializeField] private float _rotationDuration = default;
		[SerializeField] private float _rotationSize = 45;

		#endregion

		#region Private properties

		private bool _isRotating = false;

		#endregion

		#region Unity events

		private void Start()
		{
			Init();
		}

		private void OnDestroy()
		{
			Teardown();
		}

		#endregion

		#region Private methods

		private void Init()
		{
			_btnR.onClick.AddListener(RotateToRight);
			_btnL.onClick.AddListener(RotateToLeft);
		}

		private void Teardown()
		{
			_btnR.onClick.RemoveAllListeners();
			_btnL.onClick.RemoveAllListeners();
		}

		private void RotateToRight()
		{
			if (_isRotating) return;

			_isRotating = true;

			StartCoroutine(Rotate(true));
		}

		private void RotateToLeft()
		{
			if (_isRotating) return;

			_isRotating = true;

			StartCoroutine(Rotate(false));
		}

		private IEnumerator Rotate(bool toRight)
		{
			var size = (!toRight) ? _rotationSize : (-1) * _rotationSize;
			var from = _cameraPivot.rotation;
			
			var eulerTo = from.eulerAngles;
			eulerTo.y += size;

			var to = Quaternion.Euler(eulerTo);
			
			float elapsed = 0.0f;

			while (elapsed < _rotationDuration)
			{
				_cameraPivot.rotation = Quaternion.Slerp(from, to, elapsed / _rotationDuration);
				
				elapsed += Time.deltaTime;
				
				yield return null;
			}

			_cameraPivot.rotation = to;

			yield return null;

			OnRotationCompleted();
		}

		private void OnRotationCompleted()
		{
			_isRotating = false;

			// TODO: Communicate rotation completion if needed
		}

		#endregion
	}
}