/*
 * Date: January 18th, 2024
 * Author: Peche
 */

using LittleLooters.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    /// <summary>
    /// Class in charge of representing the crafting process completed panel.
    /// </summary>
    public class UI_Crafting_ProcessCompletedPanel : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private GameObject _content = default;
		[SerializeField] private Image _icon = default;
		[SerializeField] private UI_PanelAnimation _panelAnimation = default;
		[SerializeField] private PlayerCraftingService _craftingService = default;

		#endregion

		#region Private properties

		private Canvas _canvas = default;
		private const float _delayToHide = 3f;

		#endregion

		#region Unity events

		private void Awake()
		{
			_canvas = GetComponent<Canvas>();

			PlayerProgressEvents.OnCraftingAreaProcessCompleted += HandleProcessCompleted;
			UI_GameplayEvents.OnCraftingClaimed += HandleCraftingClaimed;

			HideContent();
		}

		private void OnDestroy()
		{
			PlayerProgressEvents.OnCraftingAreaProcessCompleted -= HandleProcessCompleted;
			UI_GameplayEvents.OnCraftingClaimed -= HandleCraftingClaimed;
		}

		#endregion

		#region Private methods

		private void HideContent()
		{
			_content.SetActive(false);
			_canvas.enabled = false;
		}

		private void ShowContent()
		{
			_content.SetActive(true);
			_canvas.enabled = true;

			_panelAnimation.Animate();
		}

		private void HandleProcessCompleted(PlayerProgress_CraftingAreaData craftingAreaData)
		{
			CancelInvoke(nameof(HideContent));

			var areaId = craftingAreaData.id;
			var configuration = _craftingService.GetConfigurationAreaData(areaId);

			_icon.sprite = configuration.ResourceGenerated.Icon;

			ShowContent();

			Invoke(nameof(HideContent), _delayToHide);
		}

		private void HandleCraftingClaimed(int areaId)
		{
			CancelInvoke(nameof(HideContent));

			HideContent();
		}

		#endregion
	}
}