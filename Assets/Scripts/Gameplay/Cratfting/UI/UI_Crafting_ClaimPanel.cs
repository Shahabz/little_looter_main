/*
 * Date: December 16th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_Crafting_ClaimPanel : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private GameObject _content = default;
        [SerializeField] private Button _btn = default;
        [SerializeField] private Image _icon = default;
        [SerializeField] private TMPro.TextMeshProUGUI _txtAmount = default;

        #endregion

        #region Private properties

        private int _areaId = default;

		#endregion

		#region Unity events

		private void OnEnable()
        {
            _btn.onClick.AddListener(Process);
        }

        private void OnDisable()
        {
            _btn.onClick.RemoveAllListeners();
        }

		#endregion

		#region Public methods

		public void Show(CraftingConfigurationData data, int amount)
        {
            _areaId = data.Id;

            _icon.sprite = data.ResourceGenerated.Icon;

            _txtAmount.text = $"x{amount}";

            _content.SetActive(true);
        }

        public void Hide()
        {
            _content.SetActive(false);
        }

		#endregion

		#region Private methods

        private void Process()
		{
            // TODO: sfx

            UI_GameplayEvents.OnCraftingClaimed?.Invoke(_areaId);
        }

		#endregion
	}
}