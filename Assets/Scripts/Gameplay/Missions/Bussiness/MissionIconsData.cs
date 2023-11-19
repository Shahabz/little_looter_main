/*
 * Date: November 18th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
    [System.Serializable]
    public struct MissionIconData
	{
        public MissionType type;
        public Sprite icon;
	}

    [CreateAssetMenu(fileName = "MissionIconsData", menuName = "ScriptableObjects/Missions/MissionIconsData", order = 1)]
    public class MissionIconsData : ScriptableObject
    {
        public Sprite iconNone = default;
        public MissionIconData[] icons;

        private bool _initialized = false;
        private System.Collections.Generic.Dictionary<MissionType, Sprite> _iconsByType = default;

		private void OnEnable()
		{
            _initialized = false;
		}

		public Sprite GetIcon(MissionType type)
		{
            if (!_initialized)
			{
                InitDictionary();
			}

            if (!_iconsByType.ContainsKey(type)) return iconNone;

            return _iconsByType[type];
		}

        private void InitDictionary()
		{
            var amount = icons.Length;

            _iconsByType = new System.Collections.Generic.Dictionary<MissionType, Sprite>(amount);

			for (int i = 0; i < amount; i++)
			{
                var iconData = icons[i];

                _iconsByType.Add(iconData.type, iconData.icon);
			}

            _initialized = true;
		}
    }
}