using UnityEngine;

namespace Game.GridSystem
{
    public interface IGrid
    {
        Rect Area { get; }
        Vector2 CellToWorld(Vector2Int cell);
    }
}
