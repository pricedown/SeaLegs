using System;
using UnityEngine;

namespace SeaLegs
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public enum EDemoState
        {
            ControllingPlayer,
            ControllingShip
        }

        private EDemoState demoState;

        public delegate void StateChanged(EDemoState oldState, EDemoState newState);
        public StateChanged OnStateChanged;

        public EDemoState CurrentState { get { return demoState; } }

        private void Awake()
        {
            instance = this;
        }
        
        public void ChangeGameState(EDemoState newState)
        {
            EDemoState oldState = instance.demoState;

            instance.demoState = newState;
            OnStateChanged?.Invoke(oldState, newState);
        }
    }
}
