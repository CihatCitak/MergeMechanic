using UnityEngine;

namespace Game.GridSystem
{
    public interface IGridObjectHolder
    {
        bool TryReserveRandomEmptyCell(out Vector2Int cell);
        void Occupy(Vector2Int cell, IGridObject gridObject);
    }
}
