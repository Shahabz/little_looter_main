
using UnityEngine;

public class Weapon : MonoBehaviour
{
	#region Inspector

	[Header("Muzzle")]
    [SerializeField] private Transform _muzzlePivot = default;
    [SerializeField] private GameObject _muzzlePrefab = default;

    [Header("Shells")]
    [SerializeField] private bool _hasShells = default;
    [SerializeField] private Transform _shellsPivot = default;
    [SerializeField] private GameObject _shellsPrefab = default;

    [Header("Projectile")]
    [SerializeField] private Transform _projectilePivot = default;
    [SerializeField] private GameObject _projectilePrefab = default;
    [SerializeField] private float _force = default;

	#endregion

	#region Public methods

	public void Fire()
	{
        // Muzzle
        Instantiate(_muzzlePrefab, _muzzlePivot.position, _muzzlePivot.rotation);

        // Shells
        if (_hasShells)
        {
            Instantiate(_shellsPrefab, _shellsPivot.position, _shellsPivot.rotation);
        }

        // Projectile
        var projectile = Instantiate(_projectilePrefab, _projectilePivot.position, _projectilePivot.rotation).GetComponent<ECExplodingProjectile>();

        var projectileRb = projectile.GetComponent<Rigidbody>();

        projectileRb.AddForce(_projectilePivot.forward * _force);

        projectile.DestroyInTime(5);
    }

	#endregion
}
