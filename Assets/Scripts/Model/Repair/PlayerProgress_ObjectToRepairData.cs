/*
 * Date: October 22th, 2023
 * Author: Peche
 */

using LittleLooters.Gameplay;
using System;

namespace LittleLooters.Model
{
    [Serializable]
    public struct PartProgress
	{
        public int id;
        public int amount;
        public int total;
	}

    [Serializable]
    public struct PlayerProgress_ObjectToRepairData
    {
        public int id;
        public PartProgress[] progress;
        public bool isRepairing;
        public long startRepairing;
        public float expiration;
        public bool wasRepaired;
        public int duration;

        public void Setup(RepairObjectData data)
        {
            var parts = new System.Collections.Generic.List<PartProgress>(data.Parts.Length);

            id = data.Id;

            wasRepaired = false;
            isRepairing = false;
            startRepairing = 0;

			for (int i = 0; i < data.Parts.Length; i++)
			{
                var partData = data.Parts[i];
                var partProgress = new PartProgress()
                {
                    id = partData.resourceData.Id,
                    amount = 0,
                    total = partData.amount
                };

                parts.Add(partProgress);
			}

            progress = parts.ToArray();

            duration = data.Duration;
        }

        public void AddPartsTo(int partId, int amount)
		{
			for (int i = 0; i < progress.Length; i++)
			{
                var part = progress[i];

                if (part.id != partId) continue;

                part.amount += amount;

                progress[i] = part;

                break;
			}
		}

        public PartProgress GetPartProgress(int partId)
		{
            for (int i = 0; i < progress.Length; i++)
            {
                var part = progress[i];

                if (part.id != partId) continue;

                return part;
            }

            return new PartProgress();
        }

        public bool AllPartsCompleted()
		{
			for (int i = 0; i < progress.Length; i++)
			{
                var partProgress = progress[i];

                if (partProgress.amount < partProgress.total) return false;
			}

            return true;
		}

        public int Fix(int id, int amount)
		{
            var progress = GetPartProgress(id);

            var remaining = progress.total - progress.amount;

            var toConsume = Math.Min(amount, remaining);

            AddPartsTo(id, toConsume);

            return toConsume;
		}
    }
}