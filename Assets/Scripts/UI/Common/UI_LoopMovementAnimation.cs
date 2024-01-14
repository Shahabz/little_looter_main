/*
 * Date: January 14th, 2024
 * Author: Peche
 */

using DG.Tweening;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
	public class UI_LoopMovementAnimation : MonoBehaviour
	{
		#region Inspector

		[SerializeField] private Transform _animatedObject = default;
		[SerializeField] private float _animDuration = default;
		[SerializeField] private float _delay = default;
		[SerializeField] private float _animationOffsetHorizontal = default;
		[SerializeField] private float _animationOffsetVertical = default;
		[SerializeField] private bool _autoInit = true;

		#endregion

		#region Private properties

		private bool _tweenInitialized = false;
		private Sequence _tweenSequence = default;

		#endregion

		#region Unity events

		private void Start()
		{
			if (!_autoInit) return;

			InitTween();
		}

		private void OnDestroy()
		{
			if (!_tweenInitialized) return;

			_tweenSequence.Pause();
		}

		private void OnDisable()
		{
			if (!_tweenInitialized) return;

			_tweenSequence.Pause();
		}

		private void OnEnable()
		{
			_tweenSequence.Restart();
		}

		#endregion

		#region Private methods

		private void InitTween()
		{
			var originalPosition = _animatedObject.localPosition;
			var goal = originalPosition;

			goal.x -= _animationOffsetHorizontal;
			goal.y += _animationOffsetVertical;

			_tweenSequence = DOTween.Sequence()
								.Append(_animatedObject.DOLocalMove(goal, _animDuration).SetDelay(_delay))
								.Append(_animatedObject.DOLocalMove(originalPosition, _animDuration).SetDelay(_delay))
								.SetLoops(-1, LoopType.Restart);

			_tweenSequence.Restart();

			_tweenInitialized = true;
		}

		#endregion
	}
}