using System;
using System.Collections.Generic;
using Game.GridSystem;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Tests
{
    public class GridObjectHolderTests
    {
        private class GridObject : IGridObject { }

        private GridObjectHolder _holder;

        [SetUp]
        public void SetUp()
        {
            _holder = new GameObject(nameof(GridObjectHolderTests)).AddComponent<GridObjectHolder>();
            _holder.Allocate(new Vector2Int(2, 2));
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_holder.gameObject);
        }

        [Test]
        public void TryReserveRandomEmptyCell_NeverPicksReservedOrOccupiedCell()
        {
            var picked = new HashSet<Vector2Int>();
            for (var i = 0; i < 4; i++)
            {
                Assert.IsTrue(_holder.TryReserveRandomEmptyCell(out var cell));
                Assert.IsTrue(picked.Add(cell));
                if (i % 2 == 0)
                    _holder.Occupy(cell, new GridObject());
            }
        }

        [Test]
        public void TryReserveRandomEmptyCell_NoEmptyCell_ReturnsFalse()
        {
            for (var i = 0; i < 4; i++)
                _holder.TryReserveRandomEmptyCell(out _);

            Assert.IsFalse(_holder.TryReserveRandomEmptyCell(out _));
        }

        [Test]
        public void Occupy_UnreservedCell_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => _holder.Occupy(Vector2Int.zero, new GridObject()));
        }
    }
}
