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
		}

		private void OnDestroy()
		{
			OnAnimate -= SpawnResources;
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

				//resource.transform.DOScale(Vector3.one, _delayBeforeAnimation / 2).SetEase(Ease.InBounce);

				StartCoroutine(AnimateResource(resource, position));
			}
		}

        private IEnumerator AnimateResource(Image resource, Vector3 position)
		{
			yield return new WaitForSeconds(_delayBeforeAnimation);

			var duration = Random.Range(_animationDurationMin, _animationDurationMax);

			resource.transform.DOScale(Vector3.zero, duration).SetDelay(_scaleAnimationToHudDelay);

			resource.transform.DOMove(position, duration).
				SetEase(Ease.InFlash).
				OnComplete(
					() => {
						resource.gameObject.SetActive(false);
						_readyImages.Enqueue(resource);
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