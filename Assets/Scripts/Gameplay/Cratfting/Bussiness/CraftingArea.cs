/*
 * Date: December 14th, 2023
 * Author: Peche
 */

using LittleLooters.General;
using LittleLooters.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay
{
    public class CraftingArea : MonoBehaviour
    {
        [SerializeField] private CraftingConfigurationData _data = default;
		[SerializeField] private Image _uiSelection = default;
		[SerializeField] private GameObject _uiCompleted = default;
		[SerializeField] private Image _resourceIcon = default;

		public int Id => _data.Id;
		public CraftingConfigurationData Data => _data;

		private bool _shouldClaim = false;

		#region Unity events

		private void Awake()
		{
			HideCompletedPanel();
		}

		private void OnEnable()
		{
			PlayerProgressEvents.OnCraftingAreaProcessCompleted += HandleOnCraftingAreaProcessCompleted;
			PlayerProgressEvents.OnCraftingAreaProcessClaimed += HandleOnCraftingAreaProcessClaimed;
		}

		private void OnDisable()
		{
			PlayerProgressEvents.OnCraftingAreaProcessCompleted -= HandleOnCraftingAreaProcessCompleted;
			PlayerProgressEvents.OnCraftingAreaProcessClaimed -= HandleOnCraftingAreaProcessClaimed;
		}

		#endregion

		#region Public methods

		public void StartInteraction()
		{
			if (_canDebug) DebugStartDetection();

			PlayerCraftingEvents.OnStartAreaInteraction?.Invoke(_data);

			_uiSelection.enabled = true;

			HideCompletedPanel();
		}

		public void StopInteraction()
		{
			if (_canDebug) DebugStopDetection();

			PlayerCraftingEvents.OnStopAreaInteraction?.Invoke();

			_uiSelection.enabled = false;

			if (_shouldClaim)
			{
				ShowCompletedPanel();
			}
		}

		#endregion

		#region Private methods

		private void HandleOnCraftingAreaProcessCompleted(PlayerProgress_CraftingAreaData data)
		{
			if (data.id != _data.Id) return;

			_shouldClaim = true;

			ShowCompletedPanel();
		}

		private void HandleOnCraftingAreaProcessClaimed(PlayerProgress_CraftingAreaData data)
		{
			if (_canDebug) DebugClaiming(data);

			if (data.id != _data.Id) return;

			_shouldClaim = false;
		}

		private void ShowCompletedPanel()
		{
			_uiCompleted.SetActive(true);

			_resourceIcon.sprite = _data.ResourceGenerated.Icon;
		}

		private void HideCompletedPanel()
		{
			_uiCompleted.SetActive(false);
		}

		#endregion

		#region Debug

		private bool _canDebug = false;

		private void DebugStartDetection()
		{
			Debug.LogError($"Area <color=yellow>{_data.Id}</color> was <color=green>detected</color>");
		}

		private void DebugStopDetection()
		{
			Debug.LogError($"Area <color=yellow>{_data.Id}</color> stop <color=red>detection</color>");
		}

		private void DebugClaiming(PlayerProgress_CraftingAreaData data)
		{
			Debug.LogError($"Crafting <color=orange>claimed</color> -> id: {data.id}, my id: {_data.Id}");
		}

		#endregion
	}
}