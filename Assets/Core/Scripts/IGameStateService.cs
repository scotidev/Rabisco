namespace Rabisco.Core
{
    /// <summary>
    /// Represents the current state of the game.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Cutscene,
        CameraTransition,
        Victory
    }


    /// <summary>
    /// Game state service. Manages the current state of the game.
    /// All other systems read CurrentState to decide their behavior.
    /// </summary>
    public interface IGameStateService : IGameService
    {
        /// <summary>
        /// The current state of the game.
        /// </summary>
        GameState CurrentState { get; }

        /// <summary>
        /// Transitions the game to a new state.
        /// </summary>
        /// <param name="newState">The state to transition to.</param>
        void SetState(GameState newState);
    }
}
