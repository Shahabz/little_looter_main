/*
 * Date: November 15th, 2023
 * Author: Peche
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LittleLooters.Gameplay.UI
{
    public class UI_CurrentMissionPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _txtDetail = default;
        [SerializeField] private TextMeshProUGUI _txtProgress = default;
        [SerializeField] private Image _fillProgress = default;

		private void Awake()
		{
			PlayerMissionsEvents.OnInitialization += Initialization;
			PlayerMissionsEvents.OnMoveToMission += MoveToMission;
			PlayerMissionsEvents.OnMissionProgress += MissionProgress;
		}

		private void OnDestroy()
		{
			PlayerMissionsEvents.OnInitialization -= Initialization;
			PlayerMissionsEvents.OnMoveToMission -= MoveToMission;
			PlayerMissionsEvents.OnMissionProgress -= MissionProgress;
		}

		private void Initialization(MissionConfigurationData data)
		{
			RefreshMission(data);
		}

		private void MoveToMission(MissionConfigurationData data)
		{
			// TODO: show completion of previous and then refresh it

			RefreshMission(data);
		}

		private void MissionProgress(int current, int total)
		{
			RefreshProgress(current, total);
		}

		private void RefreshMission(MissionConfigurationData data)
		{
			_txtDetail.text = data.Description;

			var goal = data.GetProgressGoal();

			RefreshProgress(0, goal);
		}

		private void RefreshProgress(int current, int goal)
		{
			var progress = (float)current / goal;

			_txtProgress.text = $"{current}/{goal}";

			_fillProgress.fillAmount = progress;
		}
	}
}