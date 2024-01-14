/*
 * Date: January 14th, 2024
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    /// <summary>
    /// Shows the circle image that represents the current player's weapon radius.
    /// When player's weapon changes the image is resized based on current radius detection.
    /// </summary>
    public class UI_Player_WeaponRadiusPanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private WeaponController _weaponController = default;
        [SerializeField] private GameObject _panel = default;
        [SerializeField] private RectTransform _area = default;

		#endregion

		#region Private properties

		private const float BASE_SIZE = 100;

		#endregion

		#region Unity events

		private void Awake()
		{
            UI_GameplayEvents.OnWeaponRadiusChanged += HandleWeaponRadiusChanged;

            LevelEnemies.OnStartDetection += HandleStartEnemiesDetection;
            LevelEnemies.OnStopDetection += HandleStopEnemiesDetection;
        }

		private void Start()
		{
            HidePanel();
		}

		private void OnDestroy()
		{
            UI_GameplayEvents.OnWeaponRadiusChanged -= HandleWeaponRadiusChanged;

            LevelEnemies.OnStartDetection -= HandleStartEnemiesDetection;
            LevelEnemies.OnStopDetection -= HandleStopEnemiesDetection;
        }

		#endregion

		#region Private methods

        private void HidePanel()
		{
            _panel.SetActive(false);
		}

        private void ShowPanel()
		{
            _panel.SetActive(true);
		}

		private void HandleStopEnemiesDetection()
		{
            HidePanel();
		}

		private void HandleStartEnemiesDetection()
		{
            var radius = _weaponController.WeaponRadiusDetection;

            RefreshRadius(radius);

            ShowPanel();
		}

		private void HandleWeaponRadiusChanged(float radius)
		{
            RefreshRadius(radius);
		}

        private void RefreshRadius(float radius)
		{
            var size = Vector2.one * radius * BASE_SIZE;

            _area.sizeDelta = size;
        }

		#endregion
	}
}