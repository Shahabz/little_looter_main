/*
 * Date: December 29th, 2023
 * Author: Peche
 */

using LittleLooters.Model;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
	public class UI_CurrentMission_RepairingSlot : MonoBehaviour
	{
		[SerializeField] private GameObject _completed = default;
		[SerializeField] private GameObject _required = default;
		[SerializeField] private Image _icon = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtRequired = default;

		private int _id = -1;
		private int _amountGoal = 0;
		private bool _wasCompleted = false;

		public int Id => _id;

		public void Setup(PartsData partData, int fixedAmount, int currentAmount)
		{
			_id = partData.resourceData.Id;

			_amountGoal = partData.amount;

			_wasCompleted = false;

			_icon.sprite = partData.resourceData.Icon;

			var amount = Mathf.Max(fixedAmount, currentAmount);

			var wasCompleted = partData.amount <= amount;

			if (wasCompleted)
			{
				MarkAsCompleted();
				return;
			}

			MarkAsRequired();

			var remainingAmount = _amountGoal - currentAmount;

			_txtRequired.text = $"{remainingAmount}";
		}

		public void Refresh(int amount)
		{
			if (_wasCompleted) return;

			_wasCompleted = _amountGoal <= amount;

			if (_wasCompleted)
			{
				MarkAsCompleted();
				return;
			}

			MarkAsRequired();

			var remainingAmount = _amountGoal - amount;

			_txtRequired.text = $"{remainingAmount}";
		}

		#region Private methods

		private void MarkAsCompleted()
		{
			_completed.SetActive(true);
			_required.SetActive(false);
		}

		private void MarkAsRequired()
		{
			_completed.SetActive(false);
			_required.SetActive(true);
		}

		#endregion
	}
}