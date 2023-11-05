/*
 * Date: November 1st, 2023
 * Author: Peche
 */

using LittleLooters.General;
using LittleLooters.Model;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace LittleLooters.Gameplay.UI
{
    public class UI_PlayerResourcesPanel_Slot : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private ResourceData _data = default;
		[SerializeField] private TextMeshProUGUI _txtValue = default;
		[SerializeField] private Image _icon = default;
		[SerializeField] private float _delayToRefresh = default;

		[Header("Refresh Animation")]
		[SerializeField] private Transform _animBody = default;
		[SerializeField] private Vector3 _animPunch = default;
		[SerializeField] private float _animDuration = default;
		[SerializeField] [Range(0, 10)]private int _animVibrato = default;
		[SerializeField] [Range(0, 1)] private float _animElasticity = default;

		#endregion

		#region Unity events

		private void Start()
		{
			RefreshAmount(0);

			_icon.sprite = _data.Icon;

			//Subscribe to event resource collection
			PlayerProgressEvents.OnResourceHasChanged += OnResourceHasChanged;
		}

		private void OnDestroy()
		{
			PlayerProgressEvents.OnResourceHasChanged -= OnResourceHasChanged;
		}

		#endregion

		#region Private methods

		private void RefreshAmount(int amount)
		{
			_txtValue.text = $"{amount}";
		}

		private void OnResourceHasChanged(int id, int amount)
		{
			if (_data.Id != id) return;

			StartCoroutine(RefreshAmountWithDelay(amount));
		}

		private IEnumerator RefreshAmountWithDelay(int amount)
		{
			yield return new WaitForSeconds(_delayToRefresh);

			_animBody.DOPunchScale(_animPunch, _animDuration, _animVibrato, _animElasticity).OnComplete(() => _animBody.transform.localScale = Vector3.one);

			RefreshAmount(amount);
		}

		#endregion
	}
}