/*
 * Date: April 7th, 2024
 * Author: Peche
 */

using LittleLooters.Model;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
	/// <summary>
	/// Shows a floating message related with the level required to destroy the 
	/// object that is in contact with the player.
	/// If player's tool level is higher than object level required, the message is not shown. 
	/// </summary>
    public class UI_ToolLevelRequirement : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _content = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtLevel = default;
        [SerializeField] private TriggerPlayerProximityObject[] _targets = default;
		[SerializeField] private UI_FollowCamera _followCamera = default;

		#endregion

		#region Private properties

		private int _visibleTargetsAmount = 0;
		private int _playerToolLevel = 1;

		#endregion

		#region Unity events

		private void OnEnable()
		{
			SubscribeTargets();

			PlayerProgressEvents.OnMeleeUpgradeClaimed += HandleToolUpgradeClaimed;
		}

		private void OnDisable()
		{
			UnsubscribeTargets();

			PlayerProgressEvents.OnMeleeUpgradeClaimed -= HandleToolUpgradeClaimed;
		}

		#endregion

		#region Private methods

		private void HandleToolUpgradeClaimed(PlayerProgressEvents.MeleeUpgradeClaimedArgs args)
		{
			_playerToolLevel = args.level;
		}

		private void SubscribeTargets()
		{
			for (int i = 0; i < _targets.Length; i++)
			{
				var target = _targets[i].OnStatusChanged += HandleTargetStatusChanged;
			}
		}

		private void UnsubscribeTargets()
		{
			for (int i = 0; i < _targets.Length; i++)
			{
				var target = _targets[i].OnStatusChanged -= HandleTargetStatusChanged;
			}
		}

		private void HandleTargetStatusChanged(GameObject target, bool isVisible)
		{
			_visibleTargetsAmount = (isVisible) ? (_visibleTargetsAmount + 1) : (_visibleTargetsAmount - 1);

			var destructible = target.transform.parent.GetComponentInParent<DestructibleResourceObject>();

			if (_visibleTargetsAmount > 0)
			{
				Show();
			}
			else
			{
				Hide();
				return;
			}

			if (destructible.IsDead) return;

			_followCamera.SetTarget(destructible.transform);

			var levelRequired = destructible.LevelRequired;

			if (levelRequired <= _playerToolLevel)
			{
				Hide();
				return;
			}

			RefreshLevel(levelRequired);
		}

		private void RefreshLevel(int level)
		{
			_txtLevel.text = $"{level}";
		}

		private void Show()
		{
			if (_content.activeSelf) return;

			_content.SetActive(true);
		}

		private void Hide()
		{
			if (!_content.activeSelf) return;

			_content.SetActive(false);
		}

		#endregion
	}
}