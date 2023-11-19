/*
 * Date: November 14th, 2023
 * Author: Peche
 */

using DG.Tweening;
using LittleLooters.Model;
using System;
using TMPro;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_ToolInfoPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private Transform _container = default;
        [SerializeField] private TextMeshProUGUI _txtLevel = default;
        [SerializeField] private GameObject _upgradeCompletedPanel = default;

        [Header("Animation")]
        [SerializeField] private float _animationDuration = default;
        [SerializeField] private Vector3 _animationPunch = default;
        [SerializeField][Range(1, 10)] private int _animationVibrato = default;
        [SerializeField][Range(0, 1)] private float _animationElasticity = default;
        [SerializeField] private Ease _animationEase = default;

		#endregion

		#region Unity events

		private void Awake()
		{
            PlayerProgressEvents.OnMeleeUpgradeCompleted += UpgradeCompleted;
            PlayerProgressEvents.OnMeleeUpgradeClaimed += Claimed;

            RefreshLevel(1);    // TODO: this value should come from player's tool data

            HideUpgradeCompleted();
		}

        private void OnDestroy()
        {
            PlayerProgressEvents.OnMeleeUpgradeCompleted -= UpgradeCompleted;
            PlayerProgressEvents.OnMeleeUpgradeClaimed -= Claimed;
        }

        #endregion

        #region Private methods

        private void UpgradeCompleted()
        {
            ShowUpgradeCompleted();
        }

        private void Claimed(PlayerProgressEvents.MeleeUpgradeClaimedArgs args)
		{
            HideUpgradeCompleted();

            Animate();

            RefreshLevel(args.level);
		}

        private void Animate()
		{
            _container.DOPunchScale(_animationPunch, _animationDuration, _animationVibrato, _animationElasticity).SetEase(_animationEase);
		}

        private void RefreshLevel(int level)
		{
            _txtLevel.text = $"LVL {level}";
		}

        private void ShowUpgradeCompleted()
		{
            _upgradeCompletedPanel.SetActive(true);
		}

        private void HideUpgradeCompleted()
        {
            _upgradeCompletedPanel.SetActive(false);
        }

        #endregion

        #region Test

        [Header("Test")]
        [SerializeField] private int _levelTesting = default;

        [ContextMenu("TEST")]
		private void Test()
		{
            Animate();
            RefreshLevel(_levelTesting);
		}

		#endregion
	}
}