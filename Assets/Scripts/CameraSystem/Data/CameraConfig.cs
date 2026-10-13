using UnityEngine;

namespace Game.CameraSystem
{
    [CreateAssetMenu(menuName = "Game/Camera Config", fileName = "CameraConfig")]
    internal class CameraConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _padding = 0.3f;

        public float Padding => _padding;
    }
}
