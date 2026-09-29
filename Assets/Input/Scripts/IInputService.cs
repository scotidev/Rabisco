using System;
using UnityEngine;

namespace Rabisco.Core.InputSystem
{
    /// <summary>
    /// Input service. Provides access to player input via virtual joysticks and gesture events.
    /// </summary>
    public interface IInputService : IGameService
    {
        /// <summary>
        /// Returns the current movement vector from the left virtual joystick.
        /// </summary>
        Vector2 GetMoveVector();

        /// <summary>
        /// Event fired when a jump gesture is detected.
        /// </summary>
        event Action OnJumpPressed;

        /// <summary>
        /// Event fired when a dash gesture is detected.
        /// </summary>
        event Action OnDashTriggered;

        /// <summary>
        /// Event fired when an attack gesture is detected.
        /// </summary>
        event Action OnAttackPressed;

        /// <summary>
        /// Event fired when a swipe gesture is detected.
        /// </summary>
        /// <param name="direction">Normalized swipe direction.</param>
        event Action<Vector2> OnSwipe;

        /// <summary>
        /// Event fired when an interact gesture is detected.
        /// </summary>
        event Action OnInteractPressed;

        /// <summary>
        /// Sets the active era to translate gestures correctly.
        /// Called by GameManager when the level changes.
        /// </summary>
        /// <param name="era">Current era number (1-6).</param>
        void SetEra(int era);
    }
}
