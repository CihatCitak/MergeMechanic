using System;
using System.Collections.Generic;
using Game.Core.Services;
using UnityEngine;

namespace Game.GridSystem
{
    internal class GridObjectHolder : BaseService, IGridObjectHolder
    {
        [SerializeField] private GridConfig _gridConfig;

        private readonly List<Vector2Int> _emptyCells = new();
        private IGridObject[,] _objects;
        private bool[,] _reserved;

        protected override void BindServices()
        {
            ServiceLocator.Bind<IGridObjectHolder>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IGridObjectHolder>(this);
        }

        protected override void Initialize()
        {
            Allocate(_gridConfig.Size);
        }

        internal void Allocate(Vector2Int size)
        {
            _objects = new IGridObject[size.x, size.y];
            _reserved = new bool[size.x, size.y];
        }

        public IGridObject Get(Vector2Int cell)
        {
            return _objects[cell.x, cell.y];
        }

        public bool IsEmpty(Vector2Int cell)
        {
            return _objects[cell.x, cell.y] == null && !_reserved[cell.x, cell.y];
        }

        public void Reserve(Vector2Int cell)
        {
            if (!IsEmpty(cell))
                throw new InvalidOperationException($"Cell {cell} is not empty.");

            _reserved[cell.x, cell.y] = true;
        }

        public bool TryReserveRandomEmptyCell(out Vector2Int cell)
        {
            CollectEmptyCells();
            if (_emptyCells.Count == 0)
            {
                cell = default;
                return false;
            }

            cell = _emptyCells[UnityEngine.Random.Range(0, _emptyCells.Count)];
            _reserved[cell.x, cell.y] = true;
            return true;
        }

        public void Occupy(Vector2Int cell, IGridObject gridObject)
        {
            if (gridObject == null)
                throw new ArgumentNullException(nameof(gridObject));
            if (!_reserved[cell.x, cell.y])
                throw new InvalidOperationException($"Cell {cell} is not reserved.");

            _reserved[cell.x, cell.y] = false;
            _objects[cell.x, cell.y] = gridObject;
        }

        public void Vacate(Vector2Int cell)
        {
            if (_objects[cell.x, cell.y] == null)
                throw new InvalidOperationException($"Cell {cell} has no owner.");

            _objects[cell.x, cell.y] = null;
        }

        private void CollectEmptyCells()
        {
            _emptyCells.Clear();
            for (var x = 0; x < _objects.GetLength(0); x++)
            {
                for (var y = 0; y < _objects.GetLength(1); y++)
                {
                    var cell = new Vector2Int(x, y);
                    if (IsEmpty(cell))
                        _emptyCells.Add(cell);
                }
            }
        }
    }
}
