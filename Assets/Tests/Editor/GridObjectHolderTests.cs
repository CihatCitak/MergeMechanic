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
        public void Reserve_CellHasNoOwnerUntilOccupied()
        {
            var gridObject = new GridObject();

            _holder.Reserve(Vector2Int.one);

            Assert.IsFalse(_holder.IsEmpty(Vector2Int.one));
            Assert.IsNull(_holder.Get(Vector2Int.one));

            _holder.Occupy(Vector2Int.one, gridObject);

            Assert.AreSame(gridObject, _holder.Get(Vector2Int.one));
        }

        [Test]
        public void Vacate_OwnedCell_MakesItEmpty()
        {
            _holder.Reserve(Vector2Int.zero);
            _holder.Occupy(Vector2Int.zero, new GridObject());

            _holder.Vacate(Vector2Int.zero);

            Assert.IsTrue(_holder.IsEmpty(Vector2Int.zero));
            Assert.IsNull(_holder.Get(Vector2Int.zero));
        }

        [Test]
        public void Reserve_NotEmptyCell_Throws()
        {
            _holder.Reserve(Vector2Int.zero);

            Assert.Throws<InvalidOperationException>(() => _holder.Reserve(Vector2Int.zero));
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
        public void CellOf_OccupiedObject_ReturnsItsCell()
        {
            var gridObject = new GridObject();
            _holder.Reserve(Vector2Int.one);
            _holder.Occupy(Vector2Int.one, gridObject);

            Assert.AreEqual(Vector2Int.one, _holder.CellOf(gridObject));
        }

        [Test]
        public void CellOf_VacatedObject_Throws()
        {
            var gridObject = new GridObject();
            _holder.Reserve(Vector2Int.zero);
            _holder.Occupy(Vector2Int.zero, gridObject);
            _holder.Vacate(Vector2Int.zero);

            Assert.Throws<InvalidOperationException>(() => _holder.CellOf(gridObject));
        }

        [Test]
        public void Occupy_UnreservedCell_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => _holder.Occupy(Vector2Int.zero, new GridObject()));
        }
    }
}
