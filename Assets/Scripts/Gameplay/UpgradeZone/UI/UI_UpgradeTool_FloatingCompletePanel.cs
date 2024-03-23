/*
 * Date: March 23th, 2024
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_UpgradeTool_FloatingCompletePanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _content = default;
		[SerializeField] private Button _btnClaim = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			PlayerProgressEvents.OnMeleeUpgradeCompleted += HandleToolUpgradeCompleted;

			_btnClaim.onClick.AddListener(Claim);
		}

		private void OnDestroy()
		{
			PlayerProgressEvents.OnMeleeUpgradeCompleted -= HandleToolUpgradeCompleted;

			_btnClaim.onClick.RemoveAllListeners();
		}

		#endregion

		#region Private methods

		private void HandleToolUpgradeCompleted()
		{
			Show();
		}

		private void Claim()
		{
			// TODO: play SFX

			UI_GameplayEvents.OnClaimToolUpgrade?.Invoke();

			Hide();
		}

		private void Show()
		{
			_content.SetActive(true);
		}

		private void Hide()
		{
			_content.SetActive(false);
		}

		#endregion
	}

}