
using UnityEngine;

namespace StarterAssets
{
	public class WeaponController : MonoBehaviour
	{
		#region Public methods

		public void CheckInput(StarterAssetsInputs input)
		{
			if (!input.attack) return;

			input.attack = false;

			ProcessAttack();
		}

		#endregion

		#region Private methods

		private void ProcessAttack()
		{
			// TODO: implement fire logic

			Debug.LogError("Attack");
		}

		#endregion
	}
}