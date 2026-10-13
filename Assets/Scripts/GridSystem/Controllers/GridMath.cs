using UnityEngine;

namespace Game.GridSystem
{
    internal static class GridMath
    {
        public static bool IsInside(Vector2Int cell, Vector2Int size)
        {
            return cell.x >= 0 && cell.y >= 0 && cell.x < size.x && cell.y < size.y;
        }

        public static Vector2 CellToLocal(Vector2Int cell, Vector2Int size, float cellSize)
        {
            return new Vector2(cell.x + 0.5f - size.x * 0.5f, cell.y + 0.5f - size.y * 0.5f) * cellSize;
        }

        public static Vector2Int LocalToCell(Vector2 local, Vector2Int size, float cellSize)
        {
            return new Vector2Int(
                Mathf.FloorToInt(local.x / cellSize + size.x * 0.5f),
                Mathf.FloorToInt(local.y / cellSize + size.y * 0.5f));
        }

        public static Vector2 AreaSize(Vector2Int size, float cellSize, float padding)
        {
            return (Vector2)size * cellSize + Vector2.one * (padding * 2f);
        }
    }
}
