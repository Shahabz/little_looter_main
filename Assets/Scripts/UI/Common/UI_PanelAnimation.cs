/*
 * Date: January 18th, 2024
 * Author: Peche
 */

using DG.Tweening;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_PanelAnimation : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private Transform _animatedObject = default;
		[SerializeField] private float _duration = default;
		[SerializeField] private float _punch = default;
		[SerializeField] private int _vibrato = default;
		[SerializeField] private float _elasticity = default;
		[SerializeField] private Ease _ease = default;

		#endregion

		#region Public methods

		public void Animate()
		{
			_animatedObject.DOPunchScale(Vector3.one * _punch, _duration, _vibrato, _elasticity).SetEase(_ease);
		}

		#endregion
	}
}