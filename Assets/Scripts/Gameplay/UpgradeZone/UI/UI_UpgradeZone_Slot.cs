/*
 * Date: November 5th, 2023
 * Author: Peche
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_UpgradeZone_Slot : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private TextMeshProUGUI _txtCurrent = default;
		[SerializeField] private TextMeshProUGUI _txtRequired = default;
		[SerializeField] private Image _icon = default;
		[SerializeField] private Color _colorCompleted = default;
		[SerializeField] private Color _colorNotCompleted = default;
		[SerializeField] private Image _toggleCompleted = default;

		#endregion

		#region Public methods

		public void Setup(int currentAmount, int requiredAmount, Sprite icon)
		{
			var completed = currentAmount >= requiredAmount;

			//_txtCurrent.text = $"{currentAmount}/";
			_txtRequired.text = $"{currentAmount}/{requiredAmount}";

			//_txtRequired.color = (completed) ? _colorCompleted : _colorNotCompleted;

			_toggleCompleted.enabled = completed;

			_icon.sprite = icon;
		}

		#endregion
	}
}