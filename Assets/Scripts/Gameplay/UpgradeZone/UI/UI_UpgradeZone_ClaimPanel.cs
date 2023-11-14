/*
 * Date: November 5th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_UpgradeZone_ClaimPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _panel = default;
		[SerializeField] private Canvas _canvas = default;
		[SerializeField] private Button _btnClaim = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			PlayerProgressEvents.OnMeleeUpgradeCompleted += UpgradeCompleted;

			_btnClaim.onClick.AddListener(Claim);

			Hide();
		}

		private void OnDestroy()
		{
			PlayerProgressEvents.OnMeleeUpgradeCompleted -= UpgradeCompleted;

			_btnClaim.onClick.RemoveAllListeners();
		}

		#endregion

		#region Public methods

		public void Show()
		{
			_panel.SetActive(true);
			_canvas.enabled = true;
		}

		public void Hide()
		{
			_panel.SetActive(false);
			_canvas.enabled = false;
		}

		#endregion

		#region Private methods

		private void UpgradeCompleted()
		{
			Show();
		}

		private void Claim()
		{
			// TODO: play SFX

			UI_GameplayEvents.OnClaimToolUpgrade?.Invoke();

			Hide();
		}

		#endregion
	}
}