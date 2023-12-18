/*
 * Date: December 17th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using LittleLooters.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_GameplayCheats_ResourcesPanelSlot : MonoBehaviour
    {
		[SerializeField] private ResourceData _data = default;
        [SerializeField] private Button _btnGrant = default;
        [SerializeField] private Button _btnConsume = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtDisplayName = default;
		[SerializeField] private TMPro.TextMeshProUGUI _txtAmount = default;
		[SerializeField] private Image _icon = default;

		private void Start()
		{
			_txtDisplayName.text = _data.DisplayName;
			_icon.sprite = _data.Icon;
		}

		private void OnEnable()
		{
			PlayerProgressEvents.OnResourceHasChanged += HandleOnResourceHasChanged;

			_btnGrant.onClick.AddListener(Grant);
			_btnConsume.onClick.AddListener(Consume);
		}

		private void OnDisable()
		{
			PlayerProgressEvents.OnResourceHasChanged -= HandleOnResourceHasChanged;

			_btnGrant.onClick.RemoveAllListeners();
			_btnConsume.onClick.RemoveAllListeners();
		}

		public void Setup(PlayerProgressData progressData)
		{
			var currentAmount = progressData.GetResourceAmount(_data.Id);

			RefreshAmount(currentAmount);
		}

		private void HandleOnResourceHasChanged(int id, int amount)
		{
			if (_data.Id != id) return;

			RefreshAmount(amount);
		}

		private void RefreshAmount(int amount)
		{
			_txtAmount.text = $"[{amount}]";
		}

		private void Grant()
		{
			var args = new UI_GameplayEvents.UpdateResourceByCheatArgs()
			{ 
				resourceId = _data.Id,
				amount = 1
			};

			UI_GameplayEvents.OnGrantResourceByCheat?.Invoke(args);
		}

		private void Consume()
		{
			var args = new UI_GameplayEvents.UpdateResourceByCheatArgs()
			{
				resourceId = _data.Id,
				amount = 1
			};

			UI_GameplayEvents.OnConsumeResourceByCheat?.Invoke(args);
		}
	}
}