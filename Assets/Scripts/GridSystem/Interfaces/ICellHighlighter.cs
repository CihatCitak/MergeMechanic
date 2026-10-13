using UnityEngine;

namespace Game.GridSystem
{
    public interface ICellHighlighter
    {
        void Show(Vector2Int cell, bool isMergeable);
        void Hide();
    }
}
