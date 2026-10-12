using UnityEngine;

namespace Game.Core.Performance
{
    internal class FrameRateController : MonoBehaviour
    {
        [SerializeField] private FrameRateConfig _config;

        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = SelectFrameRate(SystemInfo.systemMemorySize, SystemInfo.processorCount, _config);
        }

        internal static int SelectFrameRate(int memoryMb, int processorCount, FrameRateConfig config)
        {
            var isCapable = memoryMb >= config.MinMemoryMb && processorCount >= config.MinProcessorCount;
            return isCapable ? config.HighFrameRate : config.LowFrameRate;
        }
    }
}
