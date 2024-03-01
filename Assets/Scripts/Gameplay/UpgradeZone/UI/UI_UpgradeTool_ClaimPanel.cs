/*
 * Date: February 29th, 2024
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_UpgradeTool_ClaimPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _content = default;
        [SerializeField] private Button _btnClaim = default;

        private void Awake()
		{
            _btnClaim.onClick.AddListener(Claim);
        }

		public void Show()
        {
            _content.SetActive(true);
        }

        public void Hide()
        {
            _content.SetActive(false);
        }

        private void Claim()
        {
            // TODO: play SFX

            UI_GameplayEvents.OnClaimToolUpgrade?.Invoke();

            Hide();
        }
    }
}