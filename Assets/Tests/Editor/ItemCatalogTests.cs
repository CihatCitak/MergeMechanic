using Game.ItemSystem;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Tests
{
    public class ItemCatalogTests
    {
        private ItemChainData _chain;
        private ItemCatalog _catalog;

        [SetUp]
        public void SetUp()
        {
            _chain = ScriptableObject.CreateInstance<ItemChainData>();
            var serializedChain = new SerializedObject(_chain);
            serializedChain.FindProperty("_id").stringValue = "coffee";
            serializedChain.ApplyModifiedPropertiesWithoutUndo();

            _catalog = ScriptableObject.CreateInstance<ItemCatalog>();
            var serializedCatalog = new SerializedObject(_catalog);
            var chains = serializedCatalog.FindProperty("_chains");
            chains.arraySize = 1;
            chains.GetArrayElementAtIndex(0).objectReferenceValue = _chain;
            serializedCatalog.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_catalog);
            Object.DestroyImmediate(_chain);
        }

        [Test]
        public void TryGet_KnownId_ReturnsChain()
        {
            Assert.IsTrue(_catalog.TryGet("coffee", out var chain));
            Assert.AreSame(_chain, chain);
        }

        [Test]
        public void TryGet_UnknownId_ReturnsFalse()
        {
            Assert.IsFalse(_catalog.TryGet("unknown", out var chain));
            Assert.IsNull(chain);
        }
    }
}
