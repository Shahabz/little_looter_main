/*
 * Date: November 14th, 2023
 * Author: Peche
 */

using DG.Tweening;
using LittleLooters.Model;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_ToolUpgradeCompletedPanel : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Transform _container = default;

        [Header("Animation In")]
        [SerializeField] private float _animationDuration = default;
        [SerializeField] private Vector3 _animationPunch = default;
        [SerializeField] [Range(1, 10)] private int _animationVibrato = default;
        [SerializeField] [Range(0, 1)] private float _animationElasticity = default;
        [SerializeField] private Ease _animationEase = default;

        [Header("Animation Out")]
        [SerializeField] private float _animationOutDelay = default;
        [SerializeField] private float _animationOutDuration = default;
        [SerializeField] private Ease _animationOutEase = default;

        #endregion

        #region Unity events

        private void Awake()
        {
            PlayerProgressEvents.OnMeleeUpgradeCompleted += UpgradeCompleted;
        }

        private void OnDestroy()
        {
            PlayerProgressEvents.OnMeleeUpgradeCompleted -= UpgradeCompleted;
        }

		#endregion

		#region Private methods

		private void UpgradeCompleted()
		{
            Animate();
		}

        private void Animate()
        {
            _container.localScale = Vector3.one;
            _container.gameObject.SetActive(true);

            _container.DOPunchScale(_animationPunch, _animationDuration, _animationVibrato, _animationElasticity).SetEase(_animationEase).OnComplete( () => AnimationOut() );
        }

        private void AnimationOut()
		{
            _container.DOScale(0, _animationOutDuration).SetDelay(_animationOutDelay).SetEase(_animationOutEase).OnComplete( () => _container.gameObject.SetActive(false) );
		}

        #endregion

        #region Test

        [ContextMenu("TEST")]
        private void Test()
        {
            Animate();
        }

        #endregion
    }
}