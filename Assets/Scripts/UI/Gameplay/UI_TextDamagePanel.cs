/*
 * Date: November 4th, 2023
 * Author: Peche
 */

using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_TextDamagePanel : MonoBehaviour
    {
		#region Events

		public static System.Action<Vector3, int> OnAnimateDamage;
		public static System.Action<Vector3, int> OnAnimateLevelRequired;

		#endregion

		#region Inspector

		[SerializeField] private Camera _camera = default;
		[SerializeField] private TextMeshProUGUI[] _txts = default;
		[SerializeField] private float _initialOffset = default;

		[Header("Animation damage configuration")]
		[SerializeField] private int _damageFontSize = 40;
		[SerializeField] private float _animationInDurationMin = default;
		[SerializeField] private float _animationInDurationMax = default;
		[SerializeField] private float _animationInOffsetMin = default;
		[SerializeField] private float _animationInOffsetMax = default;
		[SerializeField] private float _animScaleDuration = default;

		[Header("Animation level required configuration")]
		[SerializeField] private int _levelRequiredFontSize = 20;
		[SerializeField] private float _animationLevelDurationMin = default;
		[SerializeField] private float _animationLevelDurationMax = default;
		[SerializeField] private float _animationLevelOffsetMin = default;
		[SerializeField] private float _animationLevelOffsetMax = default;
		[SerializeField] private float _animationLevelScaleDuration = default;

		#endregion

		#region Private properties

		private Queue<TextMeshProUGUI> _readyTexts = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			InitializeTexts();

			OnAnimateDamage += SpawnDamage;

			OnAnimateLevelRequired += SpawnLevelRequired;
		}

		private void OnDestroy()
		{
			OnAnimateDamage -= SpawnDamage;

			OnAnimateLevelRequired -= SpawnLevelRequired;
		}

		#endregion

		#region Private methods

		private void InitializeTexts()
		{
			var amount = _txts.Length;

			_readyTexts = new Queue<TextMeshProUGUI>(amount);

			for (int i = 0; i < amount; i++)
			{
				_readyTexts.Enqueue(_txts[i]);

				_txts[i].enabled = false;
			}
		}

		private void SpawnDamage(Vector3 position, int amount)
		{
			if (_readyTexts.Count <= 0) return;

			var damageText = _readyTexts.Dequeue();

			var initialPosition = GetInitialPosition(position);

			damageText.transform.position = initialPosition;

			var goalPosition = GetGoalPosition(initialPosition);

			damageText.text = $"{amount}";
			damageText.fontSize = _damageFontSize;

			var duration = Random.Range(_animationInDurationMin, _animationInDurationMax);
			
			damageText.transform.DOMove(goalPosition, duration).
				SetEase(Ease.InFlash).
				OnComplete( 
					() => { 
						_readyTexts.Enqueue(damageText);
						damageText.transform.DOScale(Vector3.zero, _animScaleDuration);
				});

			damageText.transform.localScale = Vector3.one;

			damageText.enabled = true;
		}

		private Vector3 GetInitialPosition(Vector3 position)
		{
			var screenPosition = _camera.WorldToScreenPoint(position);

			screenPosition.y += _initialOffset;

			return screenPosition;
		}

		private Vector3 GetGoalPosition(Vector3 position)
		{
			var offset = Random.Range(_animationInOffsetMin, _animationInOffsetMax);

			position.y += offset;

			return position;
		}

		private void SpawnLevelRequired(Vector3 position, int level)
		{
			if (_readyTexts.Count <= 0) return;

			var levelRequiredText = _readyTexts.Dequeue();

			var initialPosition = GetInitialPosition(position);

			levelRequiredText.transform.position = initialPosition;

			var goalPosition = GetLevelGoalPosition(initialPosition);

			levelRequiredText.text = $"Level Required {level}";
			levelRequiredText.fontSize = _levelRequiredFontSize;

			var duration = Random.Range(_animationLevelDurationMin, _animationLevelDurationMax);

			levelRequiredText.transform.DOMove(goalPosition, duration).
				SetEase(Ease.InFlash).
				OnComplete(
					() => {
						_readyTexts.Enqueue(levelRequiredText);
						levelRequiredText.transform.DOScale(Vector3.zero, _animationLevelScaleDuration);
					});

			levelRequiredText.transform.localScale = Vector3.one;

			levelRequiredText.enabled = true;
		}

		private Vector3 GetLevelGoalPosition(Vector3 position)
		{
			var offset = Random.Range(_animationLevelOffsetMin, _animationLevelOffsetMax);

			position.y += offset;

			return position;
		}

		#endregion

		#region Testing

		[SerializeField] private int _damageTest = 123;

		[ContextMenu("TEST")]
		private void Test()
		{
			OnAnimateDamage?.Invoke(Vector3.zero, _damageTest);
		}

		#endregion
	}
}