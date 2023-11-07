/*
 * Date: November 7th, 2023
 * Author: Peche
 */

using DG.Tweening;
using LittleLooters.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_UpgradeZone_LevelUpPopup : MonoBehaviour
    {
		#region Inspector

		[SerializeField] private GameObject _content = default;
		[SerializeField] private Button _btnClose = default;
		[SerializeField] private TextMeshProUGUI _txtLevel = default;
		[SerializeField] private TextMeshProUGUI _txtCurrentDamage = default;
		[SerializeField] private TextMeshProUGUI _txtNewDamage = default;
		
		[Header("Panel animation")]
		[SerializeField] private GameObject _panel = default;
		[SerializeField] private Vector3 _animPunch = default;
		[SerializeField] private float _animDuration = default;
		[SerializeField][Range(1, 10)] private int _animVibrato = default;
		[SerializeField] private Ease _animEase = default;

		#endregion

		#region Unity events

		private void Awake()
		{
			PlayerProgressEvents.OnMeleeUpgradeClaimed += Setup;

			_btnClose.onClick.AddListener(Close);
		}

		private void OnDestroy()
		{
			PlayerProgressEvents.OnMeleeUpgradeClaimed -= Setup;

			_btnClose.onClick.RemoveAllListeners();
		}

		#endregion

		#region Private methods

		private void Setup(PlayerProgressEvents.MeleeUpgradeClaimedArgs args)
		{
			_txtLevel.text = $"LEVEL {args.level}";
			_txtCurrentDamage.text = $"{args.oldDamage}";
			_txtNewDamage.text = $"{args.newDamage}";

			_content.SetActive(true);

			AnimatePanel();
		}

		private void Close()
		{
			// TODO: play SFX

			Hide();
		}

		private void Hide()
		{
			_content.SetActive(false);
		}


		private void AnimatePanel()
		{
			_panel.transform.DOPunchScale(_animPunch, _animDuration, _animVibrato).SetEase(_animEase);
		}

		#endregion

		#region Test

		[ContextMenu("Test")]
		private void Test()
		{
			var args = new PlayerProgressEvents.MeleeUpgradeClaimedArgs()
			{
				level = 10,
				oldDamage = 300,
				newDamage = 550
			};

			Setup(args);
		}

		#endregion
	}
}