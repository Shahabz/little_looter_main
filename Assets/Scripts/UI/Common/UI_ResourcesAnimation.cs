/*
 * Date: November 1st, 2023
 * Author: Peche
 */

namespace LittleLooters.Gameplay.UI
{
	using DG.Tweening;
	using LittleLooters.General;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Class in charge of showing resource icons animations.
	/// </summary>
	public class UI_ResourcesAnimation : MonoBehaviour
    {
		public static System.Action<int, int> OnAnimate;
		public static System.Action<int> OnAnimationCompleted;
		public static System.Action<ResourceConsumptionArgs> OnAnimateResourceConsumption;

		[System.Serializable]
		public struct ResourceConsumptionArgs
		{
			public int[] ids;
			public int[] amounts;
			public Vector2 position;
		}

		[System.Serializable]
		public struct TargetHUD
		{
			public ResourceData data;
			public RectTransform hud;
		}

		#region Inspector

		[SerializeField] private TargetHUD[] _hudTargets = default;
        [SerializeField] private Image[] _images = default;
		[SerializeField] private ResourceData[] _resourcesData = default;
		[SerializeField] private Sprite _defaultIcon = default;
		[SerializeField] private float _firstAnimationDuration = default;
		[SerializeField] private float _animationDurationMin = default;
		[SerializeField] private float _animationDurationMax = default;
		[SerializeField] private float _scaleAnimationToHudDelay = default;
		[SerializeField] private float _delayBeforeAnimation = default;
		[SerializeField] private float _initialPositionAmplitude = default;
		[SerializeField] private float _consumptionScaleDuration = default;
		[SerializeField] private float _consumptionMovementDuration = default;
		[SerializeField] private float _delayBeforeConsumptionAnimation = default;

		#endregion

		#region Private properties

		private Queue<Image> _readyImages = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			InitializeLists();

			HideAllResources();

			OnAnimate += SpawnResources;
			OnAnimateResourceConsumption += HandleResourceConsumption;
		}

		private void OnDestroy()
		{
			OnAnimate -= SpawnResources;
			OnAnimateResourceConsumption -= HandleResourceConsumption;
		}

		#endregion

		#region Private methods

		private void InitializeLists()
		{
			var amount = _images.Length;

			_readyImages = new Queue<Image>(amount);

			for (int i = 0; i < amount; i++)
			{
				_readyImages.Enqueue(_images[i]);
			}
		}

		private void HideAllResources()
		{
			var amount = _images.Length;

			for (int i = 0; i < amount; i++)
			{
				_images[i].gameObject.SetActive(false);
			}
		}

		private void SpawnResources(int id, int amount)
		{
			var icon = GetIcon(id);

			var position = GetTargetPosition(id);

			for (int i = 0; i < amount; i++)
			{
				if (_readyImages.Count <= 0) break;

				var resource = _readyImages.Dequeue();

				resource.sprite = icon;

				var goalPosition = GetInitialPosition();

				resource.transform.localPosition = Vector3.zero;

				resource.transform.DOLocalMove(goalPosition, _firstAnimationDuration).SetEase(Ease.InFlash);

				resource.transform.localScale = Vector3.one;

				resource.gameObject.SetActive(true);

				StartCoroutine(AnimateResource(id, resource, position, _delayBeforeAnimation));
			}
		}

        private IEnumerator AnimateResource(int resourceId, Image resource, Vector3 position, float delayBeforeAnimation)
		{
			yield return new WaitForSeconds(delayBeforeAnimation);

			var duration = Random.Range(_animationDurationMin, _animationDurationMax);

			resource.transform.DOScale(Vector3.zero, duration).SetDelay(_scaleAnimationToHudDelay);

			resource.transform.DOMove(position, duration).
				SetEase(Ease.InFlash).
				OnComplete(
					() => {
						resource.gameObject.SetActive(false);
						_readyImages.Enqueue(resource);
						OnAnimationCompleted?.Invoke(resourceId);
					});
		}

		private IEnumerator AnimateResourceConsumption(int resourceId, Image resource, Vector3 position, float delayBeforeAnimation)
		{
			yield return new WaitForSeconds(delayBeforeAnimation);

			resource.transform.DOScale(Vector3.zero, _consumptionScaleDuration).SetDelay(_scaleAnimationToHudDelay);

			resource.transform.DOMove(position, _consumptionMovementDuration).
				SetEase(Ease.InFlash).
				OnComplete(
					() => {
						resource.gameObject.SetActive(false);
						_readyImages.Enqueue(resource);
						OnAnimationCompleted?.Invoke(resourceId);
					});
		}

		private Sprite GetIcon(int id)
		{
			for (int i = 0; i < _resourcesData.Length; i++)
			{
				var resourceData = _resourcesData[i];

				if (resourceData.Id == id) return resourceData.Icon;
			}

			return _defaultIcon;
		}

		private Vector3 GetInitialPosition()
		{
			return Random.insideUnitCircle * _initialPositionAmplitude;
		}

		private Vector3 GetTargetPosition(int id)
		{
			for (int i = 0; i < _hudTargets.Length; i++)
			{
				var info = _hudTargets[i];

				if (info.data.Id == id) return info.hud.position;
			}

			return Vector3.zero;
		}

		private void HandleResourceConsumption(ResourceConsumptionArgs args)
		{
			var resources = args.ids;
			var amounts = args.amounts;
			var position = args.position;

			for (var i=0; i< resources.Length; i++)
			{
				var id = resources[i];

				var icon = GetIcon(id);
				var amount = amounts[i];

				for (int j = 0; j < amount; j++)
				{
					if (_readyImages.Count <= 0) break;

					var resource = _readyImages.Dequeue();

					resource.sprite = icon;

					resource.transform.localPosition = GetTargetPosition(id);

					resource.transform.localScale = Vector3.one;

					resource.gameObject.SetActive(true);

					var delay = Random.Range(_delayBeforeConsumptionAnimation - 0.1f, _delayBeforeConsumptionAnimation + 0.1f);
					StartCoroutine(AnimateResourceConsumption(id, resource, position, delay));
				}
			}
		}

		#endregion

		#region Test

		[Header("TESTING")]
		[SerializeField] private ResourceData _dataToTest = default;
		[SerializeField] private int _amountToTest = 0;

		[ContextMenu("TEST")]
		public void TestAnimation()
		{
			SpawnResources(_dataToTest.Id, _amountToTest);
		}

		#endregion
	}
}