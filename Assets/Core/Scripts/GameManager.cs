using UnityEngine;

namespace Rabisco.Core
{
    /// <summary>
    /// Game manager. Controls the current state of the game.
    /// States: MainMenu, Playing, Paused, Cutscene, CameraTransition, Victory.
    /// All other managers read CurrentState to decide their behavior.
    /// </summary>
    public class GameManager : IGameStateService, IGameService
    {
        #region FIELDS

        private GameState _currentState = GameState.MainMenu;

        #endregion


        #region PROPERTIES

        public GameState CurrentState => _currentState;

        #endregion


        #region PUBLIC API

        /// <summary>
        /// Transitions the game to a new state.
        /// Logs the transition for debugging.
        /// </summary>
        /// <param name="newState">The state to transition to.</param>
        public void SetState(GameState newState)
        {
            if (_currentState == newState)
            {
                Debug.LogWarning($"[GameManager] Already in state: {newState}. Ignoring redundant transition.");
                return;
            }

            _currentState = newState;
            Debug.Log($"[GameManager] State changed to: {newState}");
        }

        #endregion
    }
}
