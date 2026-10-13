using UnityEngine;

namespace Game.SpawnerSystem
{
    [CreateAssetMenu(menuName = "Game/Spawner Config", fileName = "SpawnerConfig")]
    internal class SpawnerConfig : ScriptableObject
    {
        [SerializeField] private string _gridFullMessage = "Tahta dolu!";
        [SerializeField, Min(0f)] private float _tapPunch = 0.15f;
        [SerializeField, Min(0f)] private float _tapPunchDuration = 0.2f;

        public string GridFullMessage => _gridFullMessage;
        public float TapPunch => _tapPunch;
        public float TapPunchDuration => _tapPunchDuration;
    }
}
