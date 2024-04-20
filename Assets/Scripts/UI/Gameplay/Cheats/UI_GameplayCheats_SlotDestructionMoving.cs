/*
 * Date: April 8th, 2024
 * Author: Peche
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_GameplayCheats_SlotDestructionMoving : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Button _btnApply = default;
        [SerializeField] private TextMeshProUGUI _txtTitle = default;
        [SerializeField] private TextMeshProUGUI _txtStatus = default;
        [SerializeField] private Color _colorOn = default;
        [SerializeField] private Color _colorOff = default;

        #endregion

        #region Private properties

        private bool _status = false;

        #endregion

        #region Unity events

        private void OnEnable()
        {
            _btnApply.onClick.AddListener(Apply);

            RefreshStatus();
        }

        private void OnDisable()
        {
            _btnApply.onClick.RemoveAllListeners();
        }

        #endregion

        #region Private methods

        private void Apply()
        {
            _status = !_status;

            UI_GameplayEvents.OnDestructionMovingChangedByCheat?.Invoke(_status);

            RefreshStatus();
        }

        private void RefreshStatus()
        {
            _txtStatus.text = (_status) ? "[ON]" :  "[OFF]";
            _txtStatus.color = (_status) ? _colorOn : _colorOff;
        }

        #endregion
    }
}