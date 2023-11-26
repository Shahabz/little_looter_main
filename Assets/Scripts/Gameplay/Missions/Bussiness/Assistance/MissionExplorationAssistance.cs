/*
 * Date: November 25th, 2023
 * Author: Peche
 */

using System.Collections.Generic;
using UnityEngine;

namespace LittleLooters.Gameplay
{
	/// <summary>
	/// Logic related with exploration mission assistance.
	/// When it is triggered, it tries to detect the unique object related with the mission.
	/// If it is on screen, then the indicator should be shown, else, the offscreen indicator should be shown.
	/// </summary>
	public class MissionExplorationAssistance : MonoBehaviour
	{
		#region Inspector

		[SerializeField] private Transform _player = default;
		[SerializeField] private float _radiusDetection = default;
		[SerializeField] private LayerMask _layer = default;

		#endregion

		#region Private properties

		private Collider[] _hits = default;
		private Transform _possibleTarget = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			_hits = new Collider[100];
		}

		#endregion

		#region Public methods

		public (Transform target, Vector3 targetOffset, bool found) Process(MissionConfigurationData mission, ExplorableObjectType explorableType)
		{
			Vector3 offset = Vector3.zero;
			offset.y = 2;

			var (target, found) = GetNearestTarget(explorableType);

			if (_canDebug) DebugProcess(mission, explorableType, found);

			return (target, offset, found);
		}

		#endregion

		#region Private methods

		private (Transform target, bool found) GetNearestTarget(ExplorableObjectType type)
		{
			var amount = Physics.OverlapSphereNonAlloc(_player.position, _radiusDetection, _hits, _layer);

			if (amount > 0)
			{
				for (int i = 0; i < amount; i++)
				{
					var hit = _hits[i];

					if (!hit.transform.TryGetComponent<ExplorableObject>(out var explorableObject)) continue;

					if (explorableObject.Type != type) continue;

					_possibleTarget = explorableObject.transform;

					return (_possibleTarget, true);
				}
			}

			return (null, false);
		}

		#endregion

		#region Debug

		private bool _canDebug = false;

		private void DebugProcess(MissionConfigurationData mission, ExplorableObjectType type, bool found)
		{
			Debug.LogError($"<color=magenta>EXPLORATION</color> -> Current mission <color=yellow>'{mission.Description}'</color>, type: <color=orange>{mission.Type}</color>, explorable Type: <color=cyan>{type}</color>, found: <color=yellow>{found}</color>");

		}

		#endregion
	}
}