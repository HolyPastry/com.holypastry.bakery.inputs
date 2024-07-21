using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bakery.Inputs
{

    public static class InputServices
    {
        public static Func<Vector2> GetMousePosition;
        public static Func<Vector2> GetMouseDelta;

        public static void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        public static void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }


    }

    public static class InputEvents
    {
        public static Action<Vector2> OnMove = delegate { };
        public static Action<GameObject> OnMainAction = delegate { };

        public static Action<GameObject> OnCursorEnter = delegate { };
        public static Action<GameObject> OnCursorExit = delegate { };
        public static Action<GameObject> OnMainActionDown = delegate { };
        public static Action<GameObject> OnMainActionUp = delegate { };

        public static Action<GameObject> OnSecondaryAction = delegate { };


    }

    public class InputManager : MonoBehaviour, Inputs.IPlayerActions
    {
        private Inputs _inputs;

        private Vector3 _position;
        private GameObject _hoveredObject;
        private Camera _camera;
        private Vector2 _deltaPosition;
        private bool _mouseDown;

        private void Awake()
        {
            _inputs = new Inputs();
            _inputs.Player.SetCallbacks(this);
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            _inputs.Enable();
            InputServices.GetMousePosition = GetPosition;
            InputServices.GetMouseDelta = GetDelta;
        }

        private Vector2 GetDelta()
        {
            return _deltaPosition;
        }

        private void OnDisable()
        {
            _inputs.Disable();
            InputServices.GetMousePosition = null;
            InputServices.GetMouseDelta = null;
        }

        private Vector2 GetPosition()
        {
            return _position;
        }



        public void OnLook(InputAction.CallbackContext context)
        {
            _deltaPosition = context.ReadValue<Vector2>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var move = context.ReadValue<Vector2>();
            InputEvents.OnMove?.Invoke(move);
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            _position = context.ReadValue<Vector2>();
        }



        void FixedUpdate()
        {
            if (_camera == null) _camera = Camera.main;

            if (RaycastUtilities.PointerIsOverUI(_position, out GameObject hitObject))
            {
                if (_hoveredObject != null)
                {
                    InputEvents.OnCursorExit?.Invoke(_hoveredObject);
                }
                if (hitObject != null)
                {
                    _hoveredObject = hitObject;
                    InputEvents.OnCursorEnter?.Invoke(hitObject);
                }
                else
                    _hoveredObject = null;

                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(_position);
            if (!Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Interactable")))
            {
                if (_hoveredObject == null) return;
                InputEvents.OnCursorExit?.Invoke(_hoveredObject);
                _hoveredObject = null;
                return;
            }
            // Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject == _hoveredObject) return;

            if (_hoveredObject != null)
            {
                InputEvents.OnCursorExit?.Invoke(_hoveredObject);
            }
            _hoveredObject = hit.collider.gameObject;
            InputEvents.OnCursorEnter?.Invoke(hit.collider.gameObject);
        }

        public void OnMainAction(InputAction.CallbackContext context)
        {
            if (context.performed)
                InputEvents.OnMainAction?.Invoke(_hoveredObject);

            if (context.canceled)
            {
                if (!_mouseDown) return;
                _mouseDown = false;
                InputEvents.OnMainActionUp?.Invoke(_hoveredObject);
            }

            if (context.started)
            {
                if (_mouseDown) return;
                _mouseDown = true;
                InputEvents.OnMainActionDown?.Invoke(_hoveredObject);
            }
        }

        public void OnSecondaryAction(InputAction.CallbackContext context)
        {
            if (context.performed)
                InputEvents.OnSecondaryAction?.Invoke(_hoveredObject);
        }
    }
}
