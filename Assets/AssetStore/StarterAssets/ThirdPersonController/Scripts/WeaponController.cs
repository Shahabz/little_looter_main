
using UnityEngine;

namespace StarterAssets
{
	public class WeaponController : MonoBehaviour
	{
		[SerializeField] private Weapon _weapon = default;

		#region Public methods

		public void CheckInput(StarterAssetsInputs input)
		{
			var reloading = CheckReloading(input);

			if (reloading) return;

			CheckAttack(input);
			
		}

		#endregion

		#region Private methods

		private bool CheckReloading(StarterAssetsInputs input)
		{
			if (!input.reload) return false;

			// TODO: check if it is already reloading

			input.reload = false;

			ProcessReload();

			return true;
		}

		private void CheckAttack(StarterAssetsInputs input)
		{
			if (!input.attack) return;

			input.attack = false;

			ProcessAttack();
		}

		private void ProcessAttack()
		{
			_weapon.Fire();
		}

		private void ProcessReload()
		{
			// TODO: implement reload logic

			Debug.LogError("Reload");
		}

		#endregion
	}
}