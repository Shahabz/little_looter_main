/*
 * Date: March 18th, 2024
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Ftue.View
{
    public class UI_Ftue_messages : MonoBehaviour
    {
        [SerializeField] private GameObject _movementPanel = default;
        [SerializeField] private GameObject _destructionPanel = default;
        [SerializeField] private GameObject _killEnemyPanel = default;

        public void ShowMovementMessage()
        {
            HideAll();

            _movementPanel.SetActive(true);
        }

        public void ShowDestructionMessage()
		{
            HideAll();

            _destructionPanel.SetActive(true);
		}

        public void ShowKillEnemyMessage()
        {
            HideAll();

            _killEnemyPanel.SetActive(true);
        }

        public void HideAll()
		{
            _movementPanel.SetActive(false);
            _destructionPanel.SetActive(false);
            _killEnemyPanel.SetActive(false);
        }
    }
}