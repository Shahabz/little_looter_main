/*
 * Date: April 7th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    public class Missions_CameraTargetAssistance : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private Transform _brainCamera = default;
		[SerializeField] private GameObject _playerCamera = default;
		[SerializeField] private GameObject _camera = default;
        [SerializeField] private float _delayAtTarget = default;

		#endregion

		#region Private properties

		private Cinemachine.CinemachineVirtualCamera _virtualCamera = default;
		private Vector3 _lastPosition = Vector3.zero;
		private bool _inProgress = false;
		private bool _atTarget = false;

		#endregion

		#region Unity events

		private void Awake()
		{
			_virtualCamera = _camera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
		}

		private void OnEnable()
		{
			UI_GameplayEvents.OnMissionCameraAssistance += HandleCameraAssistance;
		}

		private void OnDisable()
		{
			UI_GameplayEvents.OnMissionCameraAssistance -= HandleCameraAssistance;
		}

		private void LateUpdate()
		{
			if (_atTarget) return;

			if (!_inProgress) return;

			CheckAtTarget();
		}

		#endregion

		#region Private methods

		private void HandleCameraAssistance(GameObject target)
		{
			_virtualCamera.Follow = target.transform;

			_camera.transform.position = _playerCamera.transform.position;
			_camera.transform.rotation = _playerCamera.transform.rotation;
			_camera.SetActive(true);

			_inProgress = true;
			_atTarget = false;
			_lastPosition = _brainCamera.position;

			UI_GameplayEvents.OnStartCameraAssistance?.Invoke();
		}

		private void CheckAtTarget()
		{
			_atTarget = _lastPosition == _brainCamera.position;

			_lastPosition = _brainCamera.position;

			if (!_atTarget) return;

			_inProgress = false;

			Invoke(nameof(StopAssistance), _delayAtTarget);
		}

		private void StopAssistance()
		{
			_camera.SetActive(false);

			_inProgress = false;

			_atTarget = false;

			UI_GameplayEvents.OnStopCameraAssistance?.Invoke();
		}

		#endregion
	}
}