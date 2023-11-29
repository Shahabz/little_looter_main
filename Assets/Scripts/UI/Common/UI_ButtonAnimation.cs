/*
 * Date: November 29th, 2023
 * Author: Peche
 */

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    [RequireComponent(typeof(Button))]
    public class UI_ButtonAnimation : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private Transform _animatedPanel = default;
		[SerializeField] private float _goal = default;
		[SerializeField] private float _duration = default;
		[SerializeField] [Range(1, 10)] private int _vibrato = default;
		[SerializeField] [Range(0, 1)] private float _elasticity = default;
		[SerializeField] private Ease _ease = default;

		#endregion

		private Button _btn = default;

		private void Awake()
		{
			_btn = GetComponent<Button>();
		}

		private void OnEnable()
		{
			_btn.onClick.AddListener(Animate);
		}

		private void OnDisable()
		{
			_btn.onClick.RemoveListener(Animate);
		}

		private void Animate()
		{
			_animatedPanel.DOPunchScale(Vector3.one * _goal, _duration, _vibrato, _elasticity).SetEase(_ease);
		}
	}
}