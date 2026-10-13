using UnityEngine;

namespace Game.Core.Performance
{
    internal class FrameRateController : MonoBehaviour
    {
        [SerializeField] private FrameRateConfig _frameRateConfig;

        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = SelectFrameRate(SystemInfo.systemMemorySize, SystemInfo.processorCount, _frameRateConfig);
        }

        internal static int SelectFrameRate(int memoryMb, int processorCount, FrameRateConfig frameRateConfig)
        {
            var isCapable = memoryMb >= frameRateConfig.MinMemoryMb && processorCount >= frameRateConfig.MinProcessorCount;
            return isCapable ? frameRateConfig.HighFrameRate : frameRateConfig.LowFrameRate;
        }
    }
}
