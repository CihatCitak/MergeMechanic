using UnityEngine;

namespace Game.GridSystem
{
    internal static class GridMath
    {
        public static Vector2 CellToLocal(Vector2Int cell, Vector2Int size, float cellSize)
        {
            return new Vector2(cell.x + 0.5f - size.x * 0.5f, cell.y + 0.5f - size.y * 0.5f) * cellSize;
        }

        public static Vector2 AreaSize(Vector2Int size, float cellSize, float padding)
        {
            return (Vector2)size * cellSize + Vector2.one * (padding * 2f);
        }
    }
}
