using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rabisco.Core.InputSystem
{
    /// <summary>
    /// Virtual joystick UI component. Creates and manages on-screen joysticks for mobile input.
    /// Left joystick controls movement, right joystick controls actions via gestures.
    /// </summary>
    public class VirtualJoystick : MonoBehaviour
    {
        #region CONSTANTS

        private const float c_JoystickRadius = 100f;
        private const float c_KnobRadius = 40f;
        private const float c_MaxTouchDistance = 150f;
        private const float c_ZoneLeftX = 0f;
        private const float c_ZoneLeftWidth = 0.25f;
        private const float c_ZoneRightX = 0.75f;
        private const float c_ZoneRightWidth = 0.25f;
        private const float c_ZoneY = 0f;
        private const float c_ZoneHeight = 0.5f;

        #endregion

        #region SERIALIZED FIELDS

        [Header("Prefabs")]
        [SerializeField, Tooltip("Prefab for the virtual joystick visual")]
        private GameObject joystickPrefab;

        [Header("Canvas")]
        [SerializeField, Tooltip("Parent canvas for the joysticks")]
        private Canvas parentCanvas;

        #endregion

        #region FIELDS

        private InputManager inputManager;
        private GameObject leftJoystick;
        private GameObject rightJoystick;
        private Image leftBaseImage;
        private Image leftKnobImage;
        private Image rightBaseImage;
        private Image rightKnobImage;
        private int leftTouchId = -1;
        private int rightTouchId = -1;
        private Vector2 leftJoystickCenter;
        private Vector2 rightJoystickCenter;

        #endregion

        #region UNITY CALLBACKS

        private void Awake()
        {
            inputManager = ServiceLocator.Get<IInputService>() as InputManager;
            if (inputManager == null)
            {
                Debug.LogError("[VirtualJoystick] InputManager not found in ServiceLocator.");
            }
            CreateJoysticks();
        }

        private void Update()
        {
            if (inputManager == null) return;

            inputManager.UpdateRightJoystickTime(Time.deltaTime);
            HandleTouchInput();
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Creates both virtual joysticks in runtime from the prefab.
        /// </summary>
        public void CreateJoysticks()
        {
            if (joystickPrefab == null)
            {
                Debug.LogError("[VirtualJoystick] Joystick prefab is not assigned.");
                return;
            }

            if (parentCanvas == null)
            {
                parentCanvas = GetComponentInParent<Canvas>();
                if (parentCanvas == null)
                {
                    Debug.LogError("[VirtualJoystick] No parent Canvas found.");
                    return;
                }
            }

            leftJoystick = Instantiate(joystickPrefab, parentCanvas.transform);
            rightJoystick = Instantiate(joystickPrefab, parentCanvas.transform);

            leftBaseImage = leftJoystick.transform.Find("Base").GetComponent<Image>();
            leftKnobImage = leftJoystick.transform.Find("Knob").GetComponent<Image>();
            rightBaseImage = rightJoystick.transform.Find("Base").GetComponent<Image>();
            rightKnobImage = rightJoystick.transform.Find("Knob").GetComponent<Image>();

            leftJoystick.SetActive(false);
            rightJoystick.SetActive(false);
        }

        #endregion

        #region PRIVATE METHODS

        private void HandleTouchInput()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector2 touchPos = touch.position;

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        OnTouchBegan(touch.fingerId, touchPos);
                        break;
                    case TouchPhase.Moved:
                        OnTouchMoved(touch.fingerId, touchPos);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        OnTouchEnded(touch.fingerId);
                        break;
                }
            }
        }

        private void OnTouchBegan(int touchId, Vector2 touchPos)
        {
            if (IsInLeftZone(touchPos) && leftTouchId == -1)
            {
                leftTouchId = touchId;
                leftJoystickCenter = touchPos;
                leftJoystick.transform.position = touchPos;
                leftJoystick.SetActive(true);
                SetJoystickVisual(leftBaseImage, leftKnobImage, touchPos, leftJoystickCenter);
            }
            else if (IsInRightZone(touchPos) && rightTouchId == -1)
            {
                rightTouchId = touchId;
                rightJoystickCenter = touchPos;
                rightJoystick.transform.position = touchPos;
                rightJoystick.SetActive(true);
                SetJoystickVisual(rightBaseImage, rightKnobImage, touchPos, rightJoystickCenter);
                inputManager.ReportRightJoystickStart(touchPos);
            }
        }

        private bool IsInLeftZone(Vector2 pos)
        {
            return pos.x >= c_ZoneLeftX
                && pos.x <= c_ZoneLeftX + (Screen.width * c_ZoneLeftWidth)
                && pos.y >= c_ZoneY
                && pos.y <= c_ZoneY + (Screen.height * c_ZoneHeight);
        }

        private bool IsInRightZone(Vector2 pos)
        {
            float minX = c_ZoneRightX * Screen.width;
            float maxX = (c_ZoneRightX + c_ZoneRightWidth) * Screen.width;
            float minY = c_ZoneY * Screen.height;
            float maxY = (c_ZoneY + c_ZoneHeight) * Screen.height;

            bool inX = pos.x >= minX && pos.x <= maxX;
            bool inY = pos.y >= minY && pos.y <= maxY;

            return inX && inY;
        }

        private void OnTouchMoved(int touchId, Vector2 touchPos)
        {
            if (touchId == leftTouchId)
            {
                SetJoystickVisual(leftBaseImage, leftKnobImage, touchPos, leftJoystickCenter);
                Vector2 direction = (touchPos - leftJoystickCenter) / c_JoystickRadius;
                if (direction.magnitude > 1f) direction = direction.normalized;
                inputManager.ReportLeftJoystick(direction);
            }
            else if (touchId == rightTouchId)
            {
                SetJoystickVisual(rightBaseImage, rightKnobImage, touchPos, rightJoystickCenter);
                inputManager.ReportRightJoystickMove(touchPos);
            }
        }

        private void OnTouchEnded(int touchId)
        {
            if (touchId == leftTouchId)
            {
                leftTouchId = -1;
                leftJoystick.SetActive(false);
                inputManager.ReportLeftJoystick(Vector2.zero);
            }
            else if (touchId == rightTouchId)
            {
                rightTouchId = -1;
                rightJoystick.SetActive(false);
                inputManager.ReportRightJoystickEnd();
            }
        }

        private void SetJoystickVisual(Image baseImage, Image knobImage, Vector2 touchPos, Vector2 center)
        {
            Vector2 offset = touchPos - center;
            if (offset.magnitude > c_MaxTouchDistance)
            {
                offset = offset.normalized * c_MaxTouchDistance;
            }
            knobImage.rectTransform.anchoredPosition = offset;
            float alpha = 0.5f + (offset.magnitude / c_MaxTouchDistance) * 0.5f;
            baseImage.color = new Color(baseImage.color.r, baseImage.color.g, baseImage.color.b, alpha);
        }

        #endregion
    }
}
