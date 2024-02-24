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
using System;
using LittleLooters.Global.ServiceLocator;

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

		#region Private properties

		private bool _animationInProgress = false;

		#endregion

		#region Public properties

		public int ResourceId => _data.Id;

		#endregion

		#region Unity events

		private void Start()
		{
			RefreshAmount(0);

			_icon.sprite = _data.Icon;

			//Subscribe to event resource collection
			UI_GameplayEvents.OnGrantResourceByCheat += HandleGrantResourceByCheat;
			UI_GameplayEvents.OnConsumeResourceByCheat += HandleConsumeResourceByCheat;
		}

		private void OnDestroy()
		{
			UI_GameplayEvents.OnGrantResourceByCheat -= HandleGrantResourceByCheat;
			UI_GameplayEvents.OnConsumeResourceByCheat -= HandleConsumeResourceByCheat;
		}

		#endregion

		#region Public methods

		public void Refresh(int amount)
		{
			if (!_animationInProgress)
			{
				_animationInProgress = true;

				_animBody.DOPunchScale(_animPunch, _animDuration, _animVibrato, _animElasticity).OnComplete(() => _animBody.transform.localScale = Vector3.one).OnComplete(() => _animationInProgress = false);
			}

			RefreshAmount(amount);
		}

		#endregion

		#region Private methods

		private void RefreshAmount(int amount)
		{
			_txtValue.text = $"{amount}";
		}

		private IEnumerator RefreshAmountWithDelay(int amount)
		{
			yield return new WaitForSeconds(_delayToRefresh);

			if (!_animationInProgress)
			{
				_animationInProgress = true;

				_animBody.DOPunchScale(_animPunch, _animDuration, _animVibrato, _animElasticity).OnComplete(() => _animBody.transform.localScale = Vector3.one).OnComplete(() => _animationInProgress = false);
			}

			RefreshAmount(amount);
		}

		private void HandleGrantResourceByCheat(UI_GameplayEvents.UpdateResourceByCheatArgs args)
		{
			if (_data.Id != args.resourceId) return;

			var amount = GetPlayerResourceAmount(args.resourceId);

			StartCoroutine(RefreshAmountWithDelay(amount));
		}

		private void HandleConsumeResourceByCheat(UI_GameplayEvents.UpdateResourceByCheatArgs args)
		{
			if (_data.Id != args.resourceId) return;

			var amount = GetPlayerResourceAmount(args.resourceId);

			StartCoroutine(RefreshAmountWithDelay(amount));
		}

		private int GetPlayerResourceAmount(int id)
		{
			var service = ServiceLocator.Current.Get<PlayerProgressDataService>();
			var playerData = service.ProgressData;

			var resourceAmount = playerData.GetResourceAmount(id);

			return resourceAmount;
		}

		#endregion
	}
}