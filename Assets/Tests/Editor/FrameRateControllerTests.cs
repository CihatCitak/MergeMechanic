using Game.Core.Performance;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests
{
    public class FrameRateControllerTests
    {
        [Test]
        public void SelectFrameRate_UsesHighOnlyWhenMemoryAndProcessorsAreEnough()
        {
            var frameRateConfig = ScriptableObject.CreateInstance<FrameRateConfig>();

            Assert.AreEqual(frameRateConfig.HighFrameRate, FrameRateController.SelectFrameRate(frameRateConfig.MinMemoryMb, frameRateConfig.MinProcessorCount, frameRateConfig));
            Assert.AreEqual(frameRateConfig.LowFrameRate, FrameRateController.SelectFrameRate(frameRateConfig.MinMemoryMb - 1, frameRateConfig.MinProcessorCount, frameRateConfig));
            Assert.AreEqual(frameRateConfig.LowFrameRate, FrameRateController.SelectFrameRate(frameRateConfig.MinMemoryMb, frameRateConfig.MinProcessorCount - 1, frameRateConfig));

            Object.DestroyImmediate(frameRateConfig);
        }
    }
}
