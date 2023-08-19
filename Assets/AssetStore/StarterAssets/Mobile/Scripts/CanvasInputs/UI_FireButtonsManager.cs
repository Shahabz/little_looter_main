
using UnityEngine;

namespace StarterAssets
{
    public class UI_FireButtonsManager : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private UIVirtualButton _btnFireLeft = default;
        [SerializeField] private UIVirtualButton _btnFireRight = default;
		[SerializeField] private StarterAssetsInputs _input = default;

		#endregion

		#region Unity events

		private void LateUpdate()
		{
			// TODO: replace this for an event like OnStartAiming / OnStopAiming
			RefreshButtons();
		}

		#endregion

		#region Private methods

		private void RefreshButtons()
		{
			var isAiming = _input.IsAiming;

			_btnFireLeft.gameObject.SetActive(isAiming);
			_btnFireRight.gameObject.SetActive(!isAiming);
		}

		#endregion
	}
}