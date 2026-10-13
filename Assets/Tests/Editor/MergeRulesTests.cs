using Game.ItemSystem;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests
{
    public class MergeRulesTests
    {
        private ItemChainData _chain;
        private ItemChainData _otherChain;

        [SetUp]
        public void SetUp()
        {
            _chain = CreateChain(3);
            _otherChain = CreateChain(3);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_chain);
            Object.DestroyImmediate(_otherChain);
        }

        [Test]
        public void Evaluate_SameChainAndLevelBelowMax_Merges()
        {
            Assert.AreEqual(MergeResult.Merge, MergeRules.Evaluate(_chain, 2, _chain, 2));
        }

        [Test]
        public void Evaluate_DifferentChain_DoesNotMerge()
        {
            Assert.AreEqual(MergeResult.DifferentChain, MergeRules.Evaluate(_chain, 1, _otherChain, 1));
        }

        [Test]
        public void Evaluate_DifferentLevel_DoesNotMerge()
        {
            Assert.AreEqual(MergeResult.DifferentLevel, MergeRules.Evaluate(_chain, 1, _chain, 2));
        }

        [Test]
        public void Evaluate_MaxLevel_DoesNotMerge()
        {
            Assert.AreEqual(MergeResult.MaxLevel, MergeRules.Evaluate(_chain, 3, _chain, 3));
        }

        private static ItemChainData CreateChain(int levelCount)
        {
            var chain = ScriptableObject.CreateInstance<ItemChainData>();
            var serializedChain = new SerializedObject(chain);
            serializedChain.FindProperty("_levels").arraySize = levelCount;
            serializedChain.ApplyModifiedPropertiesWithoutUndo();
            return chain;
        }
    }
}
