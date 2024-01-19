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
		[SerializeField] private GameObject _uiInProgress = default;
		[SerializeField] private Slider _progressBarFill = default;
		[SerializeField] private Image _progressBarResourceIcon = default;

		public int Id => _data.Id;
		public CraftingConfigurationData Data => _data;

		private bool _shouldClaim = false;
		private float _remainingTime = 0;
		private int _duration = 0;
		private bool _inProgress = false;

		#region Unity events

		private void Awake()
		{
			HideCompletedPanel();

			HideInProgressPanel();
		}

		private void OnEnable()
		{
			PlayerProgressEvents.OnCraftingAreaProcessStarted += HandleOnCraftingAreaProcessStarted;
			PlayerProgressEvents.OnCraftingAreaProcessSpeedUp += HandleOnCraftingAreaProcessSpeedUp;
			PlayerProgressEvents.OnCraftingAreaProcessCompleted += HandleOnCraftingAreaProcessCompleted;
			PlayerProgressEvents.OnCraftingAreaProcessClaimed += HandleOnCraftingAreaProcessClaimed;
		}

		private void OnDisable()
		{
			PlayerProgressEvents.OnCraftingAreaProcessStarted -= HandleOnCraftingAreaProcessStarted;
			PlayerProgressEvents.OnCraftingAreaProcessSpeedUp -= HandleOnCraftingAreaProcessSpeedUp;
			PlayerProgressEvents.OnCraftingAreaProcessCompleted -= HandleOnCraftingAreaProcessCompleted;
			PlayerProgressEvents.OnCraftingAreaProcessClaimed -= HandleOnCraftingAreaProcessClaimed;
		}

		private void Update()
		{
			if (!_inProgress) return;

			RefreshProgressBar();
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

		private void HandleOnCraftingAreaProcessStarted(PlayerProgress_CraftingAreaData data)
		{
			_remainingTime = data.expiration - Time.time;

			_duration = data.amount * _data.DurationByUnitInSecs;

			_inProgress = true;

			ShowInProgressPanel();

			RefreshProgressBar();
		}

		private void HandleOnCraftingAreaProcessSpeedUp(PlayerProgress_CraftingAreaData data)
		{
			var hasExpired = Time.time >= data.expiration;

			_remainingTime = (hasExpired) ? 0 : data.expiration - Time.time;

			RefreshProgressBar();

			if (!hasExpired) return;

			_inProgress = false;
		}

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

			HideInProgressPanel();
		}

		private void HideCompletedPanel()
		{
			_uiCompleted.SetActive(false);
		}

		private void RefreshProgressBar()
		{
			_remainingTime -= Time.deltaTime;

			var progress = 1 - (_remainingTime / _duration);

			_progressBarFill.value = progress;

			_inProgress = _remainingTime > 0;
		}

		private void HideInProgressPanel()
		{
			_uiInProgress.SetActive(false);
		}

		private void ShowInProgressPanel()
		{
			_progressBarResourceIcon.sprite = _data.ResourceGenerated.Icon;

			_uiInProgress.SetActive(true);
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