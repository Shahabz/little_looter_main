/*
 * Date: March 9th, 2024
 * Author: Peche
 */

using System.Collections;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
	public class UI_MovementMissionFtuePanel : MonoBehaviour
    {
		[SerializeField] private VariableJoystick _joystick = default;
		[SerializeField] private GameObject _joystickUI = default;

		private void Awake()
		{
			_joystick.OnChange += HandleJoystickMovement;
		}

		private IEnumerator Start()
		{
			yield return new WaitForEndOfFrame();

			_joystickUI.SetActive(true);
		}

		private void OnDestroy()
		{
			_joystick.OnChange -= HandleJoystickMovement;
		}

		private void HandleJoystickMovement(Vector2 movement)
		{
			Destroy(gameObject);
		}
	}
}