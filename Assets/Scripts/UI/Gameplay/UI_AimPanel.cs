
using UnityEngine;

public class UI_AimPanel : MonoBehaviour
{
	[SerializeField] private RectTransform _panel = default;
	[SerializeField] private Transform _target = default;
	[SerializeField] private Camera _camera = default;
	[SerializeField] private Vector2 _offset = default;

	private void Update()
	{
		FollowTarget();
	}

	private void FollowTarget()
	{
		_panel.position = RectTransformUtility.WorldToScreenPoint(_camera, _target.position) + _offset;
	}
}
