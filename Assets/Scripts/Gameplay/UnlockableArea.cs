/*
 * Date: February 22th, 2024
 * Author: Peche
 */

using System;
using System.Collections;
using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class UnlockableArea : MonoBehaviour
    {
        [SerializeField] private DestructibleResourceObject _requiredDestructible = default;
        [SerializeField] private GameObject _lockCeiling = default;
		[SerializeField] private float _delayToUnlock = default;

		private void Awake()
		{
			_requiredDestructible.OnDead += HandleObjectDestruction;
		}

		private void OnDestroy()
		{
			_requiredDestructible.OnDead -= HandleObjectDestruction;
		}

		private void HandleObjectDestruction()
		{
			StartCoroutine(Unlock());
		}

		private IEnumerator Unlock()
		{
			yield return new WaitForSeconds(_delayToUnlock);

			_lockCeiling.SetActive(false);

			Destroy(_lockCeiling);
		}
	}
}