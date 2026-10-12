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
            var config = ScriptableObject.CreateInstance<FrameRateConfig>();

            Assert.AreEqual(config.HighFrameRate, FrameRateController.SelectFrameRate(config.MinMemoryMb, config.MinProcessorCount, config));
            Assert.AreEqual(config.LowFrameRate, FrameRateController.SelectFrameRate(config.MinMemoryMb - 1, config.MinProcessorCount, config));
            Assert.AreEqual(config.LowFrameRate, FrameRateController.SelectFrameRate(config.MinMemoryMb, config.MinProcessorCount - 1, config));

            Object.DestroyImmediate(config);
        }
    }
}
