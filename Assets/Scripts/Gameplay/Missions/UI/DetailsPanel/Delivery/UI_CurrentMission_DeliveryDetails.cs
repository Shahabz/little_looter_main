/*
 * Date: December 28th, 2023
 * Author: Peche
 */

using LittleLooters.Global.ServiceLocator;
using LittleLooters.Model;
using UnityEngine;

namespace LittleLooters.Gameplay.UI
{
    public class UI_CurrentMission_DeliveryDetails : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private UI_CurrentMission_DeliverySlot _slot = default;

        #endregion

        private int _amountGoal = 0;
        private int _id = 0;

        #region Unity events

        private void OnEnable()
        {
            PlayerProgressEvents.OnResourceHasChanged += ResourceHasChanged;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        #endregion

        #region Public methods

        public void Setup(MissionDeliveryData data)
        {
            _amountGoal = data.Amount;

            _id = data.ResourceData.Id;

            _slot.Setup(data.ResourceData.Icon, _amountGoal);

            var progressDataService = ServiceLocator.Current.Get<PlayerProgressDataService>();

            var currentAmount = progressDataService.ProgressData.GetResourceAmount(data.ResourceData.Id);

            RefreshSlotStatus(currentAmount);
        }

		#endregion

		#region Private methods

		private void UnsubscribeEvents()
        {
            PlayerProgressEvents.OnResourceHasChanged -= ResourceHasChanged;
        }

        private void ResourceHasChanged(int id, int amount)
        {
            if (_id != id) return;

            RefreshSlotStatus(amount);
        }

        private void RefreshSlotStatus(int amount)
		{
            if (_amountGoal > amount)
            {
                _slot.ShowLocked();
                return;
            }

            _slot.ShowCompleted();
        }

        #endregion
    }
}