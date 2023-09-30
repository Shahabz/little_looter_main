/*
 * Date: September 30th, 2023
 * Author: Peche
 */

using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_WeaponSelectionSlot : MonoBehaviour
    {
		[SerializeField] private Button _btn = default;
		[SerializeField] private Image _border = default;
		[SerializeField] private Sprite _borderSelected = default;
		[SerializeField] private Sprite _borderNonSelected = default;

		private int _id = -1;
		private bool _isSelected = false;
		private System.Action<int> _callback = default;

		public int Id => _id;

        public void Init(int id, bool isSelected, System.Action<int> callback)
		{
			_id = id;

			_callback = callback;

			_btn.onClick.AddListener(Select);

			if (isSelected)
			{
				MarkAsSelected();
				return;
			}

			MarkAsNonSelected();
		}

        public void Teardown()
		{
			_callback = null;

			_btn.onClick.RemoveAllListeners();
		}

        public void MarkAsSelected()
		{
			_isSelected = true;

			_border.sprite = _borderSelected;
		}

		public void MarkAsNonSelected()
		{
			_isSelected = false;

			_border.sprite = _borderNonSelected;
		}

		private void Select()
		{
			// TODO: SFX

			if (_isSelected) return;

			_callback?.Invoke(_id);
		}
    }
}