using UnityEngine;

namespace Game.UI.Toast
{
    [CreateAssetMenu(menuName = "Game/Toast Config", fileName = "ToastConfig")]
    internal class ToastConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _fadeDuration = 0.2f;
        [SerializeField, Min(0f)] private float _visibleDuration = 1.2f;

        public float FadeDuration => _fadeDuration;
        public float VisibleDuration => _visibleDuration;
    }
}
