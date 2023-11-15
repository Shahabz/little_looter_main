/*
 * Date: November 15th, 2023
 * Author: Peche
 */

namespace LittleLooters.Gameplay
{
    public class PlayerMissionTriggerDestruction
    {
        private const MissionType _missionType = MissionType.DESTRUCTION;
        private int _id = -1;
        private int _amountGoal = 0;
        private int _amount = 0;
        private bool _inProgress = false;
        private System.Action _callback = null;

        public void Initialize(System.Action callback)
		{
            _callback = callback;

            DestructibleResourceEvents.OnDestroyed += ObjectDestroyed;
        }

        public void Teardown()
		{
            _callback = null;

            DestructibleResourceEvents.OnDestroyed -= ObjectDestroyed;
        }

        public void ResetStatus(MissionType type, MissionConfigurationData mission)
		{
            if (_missionType != type)
			{
                Stop();

                return;
			}

            var data = (MissionResourceDestructionData) mission;

            Start(data.Amount, data.Destructible.Id);
		}

        public void Start(int amount, int id)
		{
            _id = id;
            _amountGoal = amount;
            _amount = 0;
            _inProgress = true;
		}

        public void Stop()
		{
            _id = -1;
            _amountGoal = 0;
            _amount = 0;
            _inProgress = false;
        }

        private void ObjectDestroyed(int id)
		{
            if (!_inProgress) return;

            if (id != _id) return;

            _amount++;

            PlayerMissionsEvents.OnMissionProgress?.Invoke(_amount, _amountGoal);

            if (_amount < _amountGoal) return;

            _callback?.Invoke();
		}
    }
}