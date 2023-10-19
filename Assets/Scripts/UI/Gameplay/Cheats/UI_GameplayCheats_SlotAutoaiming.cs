/*
 * Date: October 19th, 2023
 * Author: Peche
 */

using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_GameplayCheats_SlotAutoaiming : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Button _btnApply = default;
        [SerializeField] private TMPro.TextMeshProUGUI _txtStatus = default;
        [SerializeField] private ThirdPersonController _controller = default;
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

			_controller.SetAutoaiming(_status);

			RefreshStatus();
		}

		private void RefreshStatus()
		{
			_txtStatus.text = (_status) ? "[ON]" : "[OFF]";
			_txtStatus.color = (_status) ? _colorOn : _colorOff;
		}

		#endregion
	}
}