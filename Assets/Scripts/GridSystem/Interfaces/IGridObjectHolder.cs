using UnityEngine;

namespace Game.GridSystem
{
    public interface IGridObjectHolder
    {
        IGridObject Get(Vector2Int cell);
        bool IsEmpty(Vector2Int cell);
        void Reserve(Vector2Int cell);
        bool TryReserveRandomEmptyCell(out Vector2Int cell);
        void Occupy(Vector2Int cell, IGridObject gridObject);
        void Vacate(Vector2Int cell);
    }
}
