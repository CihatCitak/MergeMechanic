using Lean.Touch;
using UnityEngine;

namespace Game.InputSystem
{
    internal class DragController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private LeanFinger _finger;
        private IDraggable _dragged;

        private void OnEnable()
        {
            LeanTouch.OnFingerDown += OnFingerDown;
            LeanTouch.OnFingerUpdate += OnFingerUpdate;
            LeanTouch.OnFingerUp += OnFingerUp;
        }

        private void OnDisable()
        {
            LeanTouch.OnFingerDown -= OnFingerDown;
            LeanTouch.OnFingerUpdate -= OnFingerUpdate;
            LeanTouch.OnFingerUp -= OnFingerUp;
        }

        private void OnFingerDown(LeanFinger finger)
        {
            if (_dragged != null || finger.StartedOverGui)
                return;

            var hit = Physics2D.OverlapPoint(WorldOf(finger));
            if (hit == null || !hit.TryGetComponent(out _dragged))
                return;

            _finger = finger;
            _dragged.BeginDrag();
        }

        private void OnFingerUpdate(LeanFinger finger)
        {
            if (finger == _finger)
                _dragged.DragTo(WorldOf(finger));
        }

        private void OnFingerUp(LeanFinger finger)
        {
            if (finger != _finger)
                return;

            _dragged.EndDrag(WorldOf(finger));
            _dragged = null;
            _finger = null;
        }

        private Vector2 WorldOf(LeanFinger finger)
        {
            return _camera.ScreenToWorldPoint(finger.ScreenPosition);
        }
    }
}
