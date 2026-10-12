using UnityEngine;

namespace Game.Core.Performance
{
    [CreateAssetMenu(menuName = "Game/Frame Rate Config", fileName = "FrameRateConfig")]
    internal class FrameRateConfig : ScriptableObject
    {
        [SerializeField, Min(1)] private int _highFrameRate = 60;
        [SerializeField, Min(1)] private int _lowFrameRate = 30;
        [SerializeField, Min(0)] private int _minMemoryMb = 3500;
        [SerializeField, Min(1)] private int _minProcessorCount = 6;

        public int HighFrameRate => _highFrameRate;
        public int LowFrameRate => _lowFrameRate;
        public int MinMemoryMb => _minMemoryMb;
        public int MinProcessorCount => _minProcessorCount;
    }
}
