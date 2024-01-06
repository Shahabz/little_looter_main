/*
 * Date: January 6th, 2024
 * Author: Peche
 */

using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

namespace LittleLooters.Gameplay.Combat
{
	public class CameraCombatSystem : MonoBehaviour
	{
		#region Inspector

		[SerializeField] private CinemachineVirtualCamera _camera = default;
		[SerializeField] private float _durationIn = default;
		[SerializeField] private float _sizeOffset = default;
		[SerializeField] private float _durationOut = default;
		[SerializeField] private float _delayForZoomIn = default;

		#endregion

		#region Private properties

		private bool _inProgress = false;
		private bool _animatingZoomOut = false;
		private bool _animatingZoomIn = false;
		private float _animationDuration = 0;
		private float _sizeGoalIn = 0;
		private float _sizeGoalOut = 0;
		private float _sizeGoal = 0;
		private float _originalSize = 0;

		#endregion

		#region Unity events

		private void Awake()
		{
			_originalSize = _camera.m_Lens.OrthographicSize;
			_sizeGoalOut = _originalSize + _sizeOffset;
		}

		private void OnEnable()
		{
			LevelEnemies.OnStartDetection += HandleStartDetection;
			LevelEnemies.OnStopDetection += HandleStopDetection;
			UI_GameplayEvents.OnCameraSizeChanged += HandleCameraSizeChanged;
		}

		private void OnDisable()
		{
			LevelEnemies.OnStartDetection -= HandleStartDetection;
			LevelEnemies.OnStopDetection -= HandleStopDetection;
			UI_GameplayEvents.OnCameraSizeChanged -= HandleCameraSizeChanged;
		}

		#endregion

		#region Private methods

		private void HandleStartDetection()
		{
			if (_inProgress)
			{
				if (_animatingZoomOut) return;

				StopCoroutine(DoAnimation());
			}

			AnimateZoomOut();
		}

		private void HandleStopDetection()
		{
			CancelInvoke(nameof(DelayZoomIn));

			Invoke(nameof(DelayZoomIn), _delayForZoomIn);
		}

		private void HandleCameraSizeChanged(float size)
		{
			_originalSize = size;
			_sizeGoalOut = _originalSize + _sizeOffset;
		}

		private void DelayZoomIn()
		{
			if (_inProgress)
			{
				if (_animatingZoomIn) return;

				StopCoroutine(DoAnimation());
			}

			AnimateZoomIn();
		}

		private void AnimateZoomOut()
		{
			CancelInvoke(nameof(DelayZoomIn));

			if (_camera.m_Lens.OrthographicSize >= _originalSize + _sizeOffset) return;

			_inProgress = true;
			_animatingZoomIn = false;
			_animatingZoomOut = true;

			if (_canDebug) DebugZoomOut();

			_animationDuration = _durationOut;
			_sizeGoal = _sizeGoalOut;

			StartCoroutine(DoAnimation());
		}

		private void AnimateZoomIn()
		{
			if (_canDebug) DebugZoomIn();

			_inProgress = true;
			_animatingZoomIn = true;
			_animatingZoomOut = false;

			_animationDuration = _durationIn;
			_sizeGoal = _originalSize;

			StartCoroutine(DoAnimation());
		}

		private IEnumerator DoAnimation()
		{
			float elapsed = 0.0f;
			var from = _camera.m_Lens.OrthographicSize;
			var to = _sizeGoal;

			if (_canDebug) DebugStartAnimation(from, to);

			while (elapsed < _animationDuration)
			{
				_camera.m_Lens.OrthographicSize = Mathf.Lerp(from, to, elapsed / _animationDuration);

				elapsed += Time.deltaTime;

				yield return null;
			}

			_camera.m_Lens.OrthographicSize = to;

			yield return null;

			AnimationCompleted();
		}

		private void AnimationCompleted()
		{
			_inProgress = false;
			_animatingZoomIn = false;
			_animatingZoomOut = false;

			if (_canDebug) DebugCompletedAnimation();
		}

		#endregion

		#region Debug

		private bool _canDebug = false;

		private void DebugZoomOut()
		{
			Debug.LogError("CameraCombatSystem::<color=yellow>ZoomOut</color>");
		}

		private void DebugZoomIn()
		{
			Debug.LogError("CameraCombatSystem::<color=orange>ZoomIn</color>");
		}

		private void DebugStartAnimation(float from, float to)
		{
			Debug.LogError($"CameraCombatSystem::<color=cyan>StartAnimation</color> -> from: {from}, to: {to}, duration: {_animationDuration}");
		}

		private void DebugInProgressAnimation(float progress, float elapsedTime)
		{
			Debug.LogError($"CameraCombatSystem::<color=magenta>InProgressAnimation</color> -> progress: {progress}, elapsedTime: {elapsedTime}");
		}

		private void DebugCompletedAnimation()
		{
			Debug.LogError("CameraCombatSystem::<color=magenta>AnimationCompleted</color>");
		}

		#endregion
	}
}