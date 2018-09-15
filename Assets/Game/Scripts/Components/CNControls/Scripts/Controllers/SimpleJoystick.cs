using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CnControls
{
    [Flags]
    public enum ControlMovementDirection
    {
        Horizontal = 0x1,
        Vertical = 0x2,
        Both = Horizontal | Vertical
    }

    /// <summary>
    /// Simple joystick class
    /// Contains logic for creating a simple joystick
    /// </summary>
    public class SimpleJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        /// <summary>
        /// Current event camera reference. Needed for the sake of Unity Remote input
        /// </summary>
        public Camera CurrentEventCamera { get; set; }

        // ------- Inspector visible variables ---------------------------------------

        /// <summary>
        /// The range in non-scaled pixels for which we can drag the joystick around
        /// </summary>
        public float MovementRange = 50f;

        /// <summary>
        /// The name of the horizontal axis for this joystick to update
        /// </summary>
        public string HorizontalAxisName = "Horizontal";

        /// <summary>
        /// The name of the vertical axis for this joystick to update
        /// </summary>
        public string VerticalAxisName = "Vertical";

        /// <summary>
        /// Should the joystick be hidden when the user releases the finger?
        /// [Space(15f)] attribute is needed only for the editor, it creates some spacing in the inspector
        /// </summary>
        [Space(15f)]
        [Tooltip("Should the joystick be hidden on release?")]
        public bool HideOnRelease;

        /// <summary>
        /// Should the joystick be moved along with the finger
        /// </summary>
        [Tooltip("Should the Base image move along with the finger without any constraints?")]
        public bool MoveBase = true;

        /// <summary>
        /// Should the joystick be moved along with the finger
        /// </summary>
        [Tooltip("Should the joystick snap to finger? If it's FALSE, the MoveBase checkbox logic will be ommited")]
        public bool SnapsToFinger = true;

        /// <summary>
        /// Joystick movement direction
        /// Specifies the axis along which it can move
        /// </summary>
        [Tooltip("Constraints on the joystick movement axis")]
        public ControlMovementDirection JoystickMoveAxis = ControlMovementDirection.Both;

        /// <summary>
        /// Image of the joystick base
        /// </summary>
        [Tooltip("Image of the joystick base")]
        public Image JoystickBase;

        /// <summary>
        /// Image of the stick itself
        /// </summary>
        [Tooltip("Image of the stick itself")]
        public Image Stick;

        /// <summary>
        /// Rect Transform of the touch zone
        /// </summary>
        [Tooltip("Touch Zone transform")]
        public RectTransform TouchZone;

        // ---------------------------------------------------------------------------

        private Vector2 _initialStickPosition;
        private Vector2 _intermediateStickPosition;
        private Vector2 _initialBasePosition;
        private RectTransform _baseTransform;
        private RectTransform _stickTransform;

        private float _oneOverMovementRange;

        protected VirtualAxis HorizintalAxis;
        protected VirtualAxis VerticalAxis;

        private void Awake()
        {
            _stickTransform = Stick.GetComponent<RectTransform>();
            _baseTransform = JoystickBase.GetComponent<RectTransform>();

            _initialStickPosition = _stickTransform.anchoredPosition;
            _intermediateStickPosition = _initialStickPosition;
            _initialBasePosition = _baseTransform.anchoredPosition;

            _stickTransform.anchoredPosition = _initialStickPosition;
            _baseTransform.anchoredPosition = _initialBasePosition;

            _oneOverMovementRange = 1f / MovementRange;

            if (HideOnRelease)
            {
                Hide(true);
            }
        }

        private void OnEnable()
        {
            // When we enable, we get our virtual axis

            HorizintalAxis = HorizintalAxis ?? new VirtualAxis(HorizontalAxisName);
            VerticalAxis = VerticalAxis ?? new VirtualAxis(VerticalAxisName);

            // And register them in our input system
            CnInputManager.RegisterVirtualAxis(HorizintalAxis);
            CnInputManager.RegisterVirtualAxis(VerticalAxis);
        }

        private void OnDisable()
        {
            // When we disable, we just unregister our axis
            // It also happens before the game object is Destroyed
            CnInputManager.UnregisterVirtualAxis(HorizintalAxis);
            CnInputManager.UnregisterVirtualAxis(VerticalAxis);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            // Unity remote multitouch related thing
            // When we feed fake PointerEventData we can't really provide a camera, 
            // it has a lot of private setters via not created objects, so even the Reflection magic won't help a lot here
            // Instead, we just provide an actual event camera as a public property so we can easily set it in the Input Helper class
            CurrentEventCamera = eventData.pressEventCamera ?? CurrentEventCamera;

            // We get the local position of the joystick
            Vector3 worldJoystickPosition;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_stickTransform, eventData.position,
                CurrentEventCamera, out worldJoystickPosition);

            // Then we change it's actual position so it snaps to the user's finger
            _stickTransform.position = worldJoystickPosition;
            // We then query it's anchored position. It's calculated internally and quite tricky to do from scratch here in C#
            var stickAnchoredPosition = _stickTransform.anchoredPosition;

            // Some bitwise logic for constraining the joystick along one of the axis
            // If the "Both" option was selected, non of these two checks will yield "true"
            if ((JoystickMoveAxis & ControlMovementDirection.Horizontal) == 0)
            {
                stickAnchoredPosition.x = _intermediateStickPosition.x;
            }
            if ((JoystickMoveAxis & ControlMovementDirection.Vertical) == 0)
            {
                stickAnchoredPosition.y = _intermediateStickPosition.y;
            }

            _stickTransform.anchoredPosition = stickAnchoredPosition;

            // Find current difference between the previous central point of the joystick and it's current position
            Vector2 difference = new Vector2(stickAnchoredPosition.x, stickAnchoredPosition.y) - _intermediateStickPosition;

            // Normalisation stuff
            var diffMagnitude = difference.magnitude;
            var normalizedDifference = difference / diffMagnitude;

            // If the joystick is being dragged outside of it's range
            if (diffMagnitude > MovementRange)
            {
                if (MoveBase && SnapsToFinger)
                {
                    // We move the base so it maps the new joystick center position
                    var baseMovementDifference = difference.magnitude - MovementRange;
                    var addition = normalizedDifference * baseMovementDifference;
                    _baseTransform.anchoredPosition += addition;
                    _intermediateStickPosition += addition;
                }
                else
                {
                    _stickTransform.anchoredPosition = _intermediateStickPosition + normalizedDifference * MovementRange;
                }
            }

            // We should now calculate axis values based on final position and not on "virtual" one
            var finalStickAnchoredPosition = _stickTransform.anchoredPosition;
            // Sanity recalculation
            Vector2 finalDifference = new Vector2(finalStickAnchoredPosition.x, finalStickAnchoredPosition.y) - _intermediateStickPosition;
            // We don't need any values that are greater than 1 or less than -1
            var horizontalValue = Mathf.Clamp(finalDifference.x * _oneOverMovementRange, -1f, 1f);
            var verticalValue = Mathf.Clamp(finalDifference.y * _oneOverMovementRange, -1f, 1f);

            // Finally, we update our virtual axis
            HorizintalAxis.Value = horizontalValue;
            VerticalAxis.Value = verticalValue;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // When we lift our finger, we reset everything to the initial state
            _baseTransform.anchoredPosition = _initialBasePosition;
            _stickTransform.anchoredPosition = _initialStickPosition;
            _intermediateStickPosition = _initialStickPosition;

            HorizintalAxis.Value = VerticalAxis.Value = 0f;

            // We also hide it if we specified that behaviour
            if (HideOnRelease)
            {
                Hide(true);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // When we press, we first want to snap the joystick to the user's finger
            if (SnapsToFinger)
            {
                CurrentEventCamera = eventData.pressEventCamera ?? CurrentEventCamera;

                Vector3 localStickPosition;
                Vector3 localBasePosition;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(_stickTransform, eventData.position,
                    CurrentEventCamera, out localStickPosition);
                RectTransformUtility.ScreenPointToWorldPointInRectangle(_baseTransform, eventData.position,
                    CurrentEventCamera, out localBasePosition);

                _baseTransform.position = localBasePosition;
                _stickTransform.position = localStickPosition;
                _intermediateStickPosition = _stickTransform.anchoredPosition;
            }
            else
            {
                OnDrag(eventData);
            }
            // We also want to show it if we specified that behaviour
            if (HideOnRelease)
            {
                Hide(false);
            }
        }

        /// <summary>
        /// Simple "Hide" behaviour
        /// </summary>
        /// <param name="isHidden">Whether the joystick should be hidden</param>
        private void Hide(bool isHidden)
        {
            JoystickBase.gameObject.SetActive(!isHidden);
            Stick.gameObject.SetActive(!isHidden);
        }
    }
}
