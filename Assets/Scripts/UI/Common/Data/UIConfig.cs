using UnityEngine;

namespace Game.UI.Common
{
    [CreateAssetMenu(menuName = "Game/UI Config", fileName = "UIConfig")]
    internal class UIConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _toggleDuration = 0.15f;
        [SerializeField, Range(0f, 1f)] private float _pressScale = 0.92f;
        [SerializeField, Min(0f)] private float _pressDuration = 0.08f;

        public float ToggleDuration => _toggleDuration;
        public float PressScale => _pressScale;
        public float PressDuration => _pressDuration;
    }
}
