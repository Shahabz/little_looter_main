/*
 * Date: March 9th, 2024
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_DestructionMissionFtuePanel : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _panel = default;
		[SerializeField] private Button _btnClose = default;
		[SerializeField] private float _delayShowButton = default;
		[SerializeField] private int _bushId = 1;
		[SerializeField] private VariableJoystick _joystick = default;

		#endregion

		private bool _firstTime = true;

		#region Unity events

		private void Awake()
		{
			ExplorableObjectEvents.OnEnter += HandleEnterExplorableObject;

			_btnClose.onClick.AddListener(Close);

			_btnClose.gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			ExplorableObjectEvents.OnEnter -= HandleEnterExplorableObject;

			_btnClose.onClick.RemoveAllListeners();
		}

		private void HandleEnterExplorableObject(ExplorableObjectType type, int id)
		{
			if (!_firstTime) return;

			if (type != ExplorableObjectType.RESOURCE) return;

			var gameConfigurationService = ServiceLocator.Current.Get<GameConfigurationService>();
			var resourceInfo = gameConfigurationService.TryGetResource(id);

			if (!resourceInfo.found) return;

			if (resourceInfo.resource.Id != _bushId) return;

			_firstTime = false;

			_panel.SetActive(true);

			_joystick.Stop();
			_joystick.gameObject.SetActive(false);

			Invoke(nameof(ShowCloseButton), _delayShowButton);
		}

		private void ShowCloseButton()
		{
			_btnClose.gameObject.SetActive(true);
		}

		private void Close()
		{
			_joystick.gameObject.SetActive(true);

			_panel.SetActive(false);

			Destroy(gameObject);
		}

		#endregion
	}
}