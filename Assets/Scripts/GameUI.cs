using UnityEngine;

namespace SeaLegs
{
    public class GameUI : MonoBehaviour
    {
        // this is a pretty tightly coupled class :(

        [SerializeField]
        private GameObject _playerUIHolder;

        [SerializeField]
        private GameObject _shipUIHolder;

        private void Start()
        {
            GameManager.instance.OnStateChanged += UpdateStateUI;

            UpdateStateUI(GameManager.instance.CurrentState, GameManager.instance.CurrentState);
        }

        /// <summary>
        /// Updates current UI based on the current game state
        /// </summary>
        private void UpdateStateUI(GameManager.EDemoState oldState, GameManager.EDemoState newState)
        {
            if (newState == GameManager.EDemoState.ControllingPlayer) ShowPlayerUI();
            else if (newState == GameManager.EDemoState.ControllingShip) ShowShipUI();
        }

        /// <summary>
        /// Displays relevant HUD for player
        /// </summary>
        private void ShowPlayerUI()
        {
            _playerUIHolder.SetActive(true);
            _shipUIHolder.SetActive(false);
        }

        /// <summary>
        /// Displays relevant HUD for controlling ship
        /// </summary>
        private void ShowShipUI()
        {
            _playerUIHolder.SetActive(false);
            _shipUIHolder.SetActive(true);
        }
    }
}
