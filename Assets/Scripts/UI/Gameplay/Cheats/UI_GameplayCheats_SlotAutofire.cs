/*
 * Date: March 18th, 2024
 * Author: Peche
 */

using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_GameplayCheats_SlotAutofire : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Button _btnApply = default;
        [SerializeField] private ThirdPersonController _controller = default;
        [SerializeField] private TextMeshProUGUI _txtTitle = default;
        [SerializeField] private TextMeshProUGUI _txtStatus = default;
        [SerializeField] private Color _colorOn = default;
        [SerializeField] private Color _colorOff = default;

        #endregion

        #region Private properties

        private int _status = 0;
        private UI_GameplayEvents.AutofireByCheatArgs _args = default;

        #endregion

        #region Public methods

        public void ApplyPreset()
        {
            _status = 1;

            Apply();
        }

        #endregion

        #region Unity events

        private void OnEnable()
        {
            _args = new UI_GameplayEvents.AutofireByCheatArgs();

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
            _status = (_status == 2) ? 0 : _status + 1;

            _args.enabled = _status > 0;
            _args.movementFree = _status == 1;

            UI_GameplayEvents.OnAutofireChangedByCheat?.Invoke(_args);

            RefreshStatus();
        }

        private void RefreshStatus()
        {
            _txtStatus.text = (_status == 0) ? "[OFF]" :  (_status == 1) ? "[FREE MOVEMENT]" : "[NO MOVEMENT]";
            _txtStatus.color = (_status > 0) ? _colorOn : _colorOff;
        }

        #endregion
    }
}