/*
 * Date: November 18th, 2023
 * Author: Peche
 */

using System.Collections.Generic;
using UnityEngine;

namespace LittleLooters.Gameplay
{
	/// <summary>
	/// Logic related with destruction mission assistance.
	/// When it is triggered, it tries to detect the nearest object related with the mission.
	/// If it is on screen, then the indicator should be shown, else, the offscreen indicator should be shown.
	/// </summary>
	public class MissionDestructionAssistance : MonoBehaviour
	{
		[SerializeField] private Transform _player = default;
		[SerializeField] private float _radiusDetection = default;
		[SerializeField] private LayerMask _layer = default;

		private Collider[] _hits = default;
		private List<Transform> _possibleTargets = default;

		private void Awake()
		{
			_hits = new Collider[100];
			_possibleTargets = new List<Transform>();
		}

		public (Transform target, Vector3 targetOffset, bool found) Process(MissionConfigurationData mission, int destructibleId)
		{
			Vector3 offset = Vector3.zero;
			offset.y = 2;

			var (target, found) = GetNearestTarget(destructibleId);

			if (_canDebug) DebugProcess(mission, destructibleId, found);

			return (target, offset, found);
		}

		private (Transform target, bool found) GetNearestTarget(int id)
		{
			var amount = Physics.OverlapSphereNonAlloc(_player.position, _radiusDetection, _hits, _layer);

			if (amount > 0)
			{
				_possibleTargets.Clear();

				for (int i = 0; i < amount; i++)
				{
					var hit = _hits[i];

					if (!hit.transform.TryGetComponent<DestructibleResourceObject>(out var destructible)) continue;

					if (destructible.Id != id) continue;

					if (destructible.IsDead) continue;

					_possibleTargets.Add(destructible.transform);
				}

				if (_possibleTargets.Count > 0)
				{
					var nearest = GetNearest();

					return (nearest, true);
				}
			}

			return (null, false);
		}

		private Transform GetNearest()
		{
			var distance = float.MaxValue;
			var nearest = _possibleTargets[0];

			for (int i = 0; i < _possibleTargets.Count; i++)
			{
				var target = _possibleTargets[i];

				var distanceToPlayer = (target.position - _player.position).sqrMagnitude;

				if (distanceToPlayer > distance) continue;

				distance = distanceToPlayer;

				nearest = target;
			}

			return nearest;
		}

		#region Debug

		private bool _canDebug = false;

		private void DebugProcess(MissionConfigurationData mission, int destructibleId, bool found)
		{
			Debug.LogError($"<color=magenta>DESTRUCTION</color> ->Current mission <color=yellow>'{mission.Description}'</color>, type: <color=orange>{mission.Type}</color>, destructible id: <color=cyan>{destructibleId}</color>, found: <color=yellow>{found}</color>");

		}

		#endregion
	}
}