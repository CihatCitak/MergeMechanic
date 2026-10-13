using UnityEngine;

namespace Game.GridSystem
{
    public interface IGrid
    {
        Vector2Int Size { get; }
        Rect Area { get; }
        bool IsInside(Vector2Int cell);
        Vector2 CellToWorld(Vector2Int cell);
        Vector2Int WorldToCell(Vector2 world);
    }
}
