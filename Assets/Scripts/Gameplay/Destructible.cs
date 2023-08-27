
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private GameObject _art = default;
    [SerializeField] private float _hp = default;
	[SerializeField] private Collider _collider = default;

	public void TakeDamage(float damage)
	{
		_hp = Mathf.Clamp(_hp - damage, 0, _hp);

		if (_hp > 0) return;

		Death();
	}

	private void Death()
	{
		_art.SetActive(false);

		_collider.enabled = false;

		Debug.LogError($"Destructible <color=yellow>{name}</color> was destroyed");
	}
}
