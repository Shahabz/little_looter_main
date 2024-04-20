/*
 * Date: April 8th, 2024
 * Author: Peche
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_GameplayCheats_SlotToolLevelUp : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Button _btnApply = default;

        #endregion

        #region Unity events

        private void OnEnable()
        {
            _btnApply.onClick.AddListener(Apply);
        }

        private void OnDisable()
        {
            _btnApply.onClick.RemoveAllListeners();
        }

        #endregion

        #region Private methods

        private void Apply()
        {
            UI_GameplayEvents.OnToolUpgradeByCheat?.Invoke();
        }

        #endregion
    }
}