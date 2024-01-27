/*
 * Date: January 27th, 2024
 * Author: Peche
 */

using LittleLooters.Gameplay.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay
{
    public class UI_EnemyHud : MonoBehaviour
    {
        [SerializeField] private GameObject _content = default;
        [SerializeField] private TMPro.TextMeshProUGUI _txtLevel = default;
        [SerializeField] private Slider _hpBar = default;

        #region Public methods
        
        public void Init(int level, int hp, int maxHp)
		{
            _txtLevel.text = $"{level}";

            var progress = (float)hp / (float)maxHp;

            RefreshHealth(progress);
        }
		
        public void RefreshHealth(float progress)
        {
            _hpBar.value = progress;
        }

        public void HandleDead()
		{
            Hide();
		}

        public void Show()
		{
            _content.SetActive(true);
		}

        public void Hide()
		{
            _content.SetActive(false);
        }

		#endregion
	}
}