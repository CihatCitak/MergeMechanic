using Lean.Touch;
using UnityEngine;

namespace Game.InputSystem
{
    internal class TapController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private void OnEnable()
        {
            LeanTouch.OnFingerDown += OnFingerDown;
        }

        private void OnDisable()
        {
            LeanTouch.OnFingerDown -= OnFingerDown;
        }

        private void OnFingerDown(LeanFinger finger)
        {
            if (finger.StartedOverGui)
                return;

            var hit = Physics2D.OverlapPoint(_camera.ScreenToWorldPoint(finger.ScreenPosition));
            if (hit != null && hit.TryGetComponent<ITappable>(out var tappable))
                tappable.Tap();
        }
    }
}
