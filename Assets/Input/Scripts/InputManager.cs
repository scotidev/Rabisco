using System;
using UnityEngine;

namespace Rabisco.Core.InputSystem
{
    /// <summary>
    /// Input manager. Processes touch input from VirtualJoystick and translates it into game actions.
    /// Gesture mapping varies by era. Values are hardcoded (no ScriptableObject).
    /// </summary>
    public class InputManager : IInputService
    {
        #region CONSTANTS

        private const float c_MoveDeadZone = 0.1f;
        private const float c_SwipeThreshold = 50f;
        private const float c_PressMaxTime = 0.2f;
        private const float c_HoldMinTime = 0.5f;

        #endregion

        #region FIELDS

        private int currentEra = 1;
        private Vector2 moveVector;
        private Vector2 rightJoystickStartPos;
        private float rightJoystickTouchTime;
        private bool rightJoystickIsTouching;
        private Vector2 rightJoystickCurrentPos;

        #endregion

        #region EVENTS

        /// <inheritdoc />
        public event Action OnJumpPressed;

        /// <inheritdoc />
        public event Action OnDashTriggered;

        /// <inheritdoc />
        public event Action OnAttackPressed;

        /// <inheritdoc />
        public event Action<Vector2> OnSwipe;

        /// <inheritdoc />
        public event Action OnInteractPressed;

        #endregion

        #region PUBLIC METHODS

        /// <inheritdoc />
        public Vector2 GetMoveVector() => moveVector;

        /// <inheritdoc />
        public void SetEra(int era)
        {
            currentEra = era;
            Debug.Log($"[InputManager] Era set to: {era}");
        }

        /// <summary>
        /// Reports a touch on the left virtual joystick.
        /// Called by VirtualJoystick when the left joystick is touched.
        /// </summary>
        /// <param name="direction">Normalized direction vector.</param>
        public void ReportLeftJoystick(Vector2 direction)
        {
            if (direction.magnitude < c_MoveDeadZone)
            {
                moveVector = Vector2.zero;
            }
            else
            {
                moveVector = direction;
            }
        }

        /// <summary>
        /// Reports the start of a touch on the right virtual joystick.
        /// Called by VirtualJoystick when the right joystick is touched.
        /// </summary>
        /// <param name="startPosition">Screen position where the touch started.</param>
        public void ReportRightJoystickStart(Vector2 startPosition)
        {
            rightJoystickStartPos = startPosition;
            rightJoystickCurrentPos = startPosition;
            rightJoystickTouchTime = 0f;
            rightJoystickIsTouching = true;
        }

        /// <summary>
        /// Reports movement of the right virtual joystick touch.
        /// Called by VirtualJoystick when the right joystick touch moves.
        /// </summary>
        /// <param name="currentPosition">Current screen position.</param>
        public void ReportRightJoystickMove(Vector2 currentPosition)
        {
            rightJoystickCurrentPos = currentPosition;
        }

        /// <summary>
        /// Reports the end of a touch on the right virtual joystick.
        /// Called by VirtualJoystick when the right joystick is released.
        /// Triggers gesture detection based on era.
        /// </summary>
        public void ReportRightJoystickEnd()
        {
            if (!rightJoystickIsTouching) return;
            rightJoystickIsTouching = false;

            Vector2 delta = rightJoystickCurrentPos - rightJoystickStartPos;
            float distance = delta.magnitude;
            float time = rightJoystickTouchTime;

            DetectGesture(delta, distance, time);
        }

        /// <summary>
        /// Updates the touch timer for the right joystick.
        /// Called by VirtualJoystick every frame while touching.
        /// </summary>
        /// <param name="deltaTime">Time since last frame.</param>
        public void UpdateRightJoystickTime(float deltaTime)
        {
            if (rightJoystickIsTouching)
            {
                rightJoystickTouchTime += deltaTime;
            }
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Detects the gesture type based on distance and time, then triggers the appropriate event.
        /// Gesture mapping varies by era.
        /// </summary>
        private void DetectGesture(Vector2 delta, float distance, float time)
        {
            if (distance >= c_SwipeThreshold)
            {
                Vector2 direction = delta.normalized;
                OnSwipe?.Invoke(direction);
                return;
            }

            if (time <= c_PressMaxTime)
            {
                TriggerPressGesture();
                return;
            }

            if (time >= c_HoldMinTime)
            {
                TriggerHoldGesture();
            }
        }

        /// <summary>
        /// Triggers the press gesture based on the current era.
        /// </summary>
        private void TriggerPressGesture()
        {
            switch (currentEra)
            {
                case 1:
                    OnJumpPressed?.Invoke();
                    break;
                case 4:
                    OnAttackPressed?.Invoke();
                    break;
                case 5:
                    OnAttackPressed?.Invoke();
                    break;
                default:
                    OnJumpPressed?.Invoke();
                    break;
            }
        }

        /// <summary>
        /// Triggers the hold gesture based on the current era.
        /// </summary>
        private void TriggerHoldGesture()
        {
            switch (currentEra)
            {
                case 1:
                    OnInteractPressed?.Invoke();
                    break;
                case 2:
                case 3:
                    OnDashTriggered?.Invoke();
                    break;
                default:
                    OnInteractPressed?.Invoke();
                    break;
            }
        }

        #endregion
    }
}
