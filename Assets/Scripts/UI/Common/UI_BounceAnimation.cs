/*
 * Date: November 29th, 2023
 * Author: Peche
 */

using DG.Tweening;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_BounceAnimation : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _animated = default;
		[SerializeField] private float _animationScale = default;
		[SerializeField] private float _animationDuration = default;

		#endregion

		#region Private properties

		private Sequence _tweenSequence = default;
		private bool _tweenCrated = false;

		#endregion

		#region Unity events

		private void OnEnable()
		{
			if (!_tweenCrated)
			{
				CreateTween();
			}

			_tweenSequence.Restart();
		}

		private void OnDisable()
		{
			_tweenSequence.Pause();
		}

		#endregion

		#region Private methods

		private void CreateTween()
		{
			_tweenSequence = DOTween.Sequence()
								.Append(_animated.transform.DOScale(Vector3.one * _animationScale, _animationDuration).SetDelay(0.1f))
								.Append(_animated.transform.DOScale(Vector2.one, _animationDuration))
								.SetLoops(-1, LoopType.Restart);

			_tweenCrated = true;
		}

		#endregion
	}
}