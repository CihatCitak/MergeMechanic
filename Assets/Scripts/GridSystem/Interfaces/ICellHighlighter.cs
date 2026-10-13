using System.Collections.Generic;
using UnityEngine;

namespace Game.GridSystem
{
    public interface ICellHighlighter
    {
        void ShowTarget(Vector2Int cell, bool isMergeable);
        void HideTarget();
        void ShowCandidates(IReadOnlyList<Vector2Int> cells);
        void HideCandidates();
    }
}
