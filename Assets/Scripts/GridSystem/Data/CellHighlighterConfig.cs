using UnityEngine;

namespace Game.GridSystem
{
    [CreateAssetMenu(menuName = "Game/Cell Highlighter Config", fileName = "CellHighlighterConfig")]
    internal class CellHighlighterConfig : ScriptableObject
    {
        [SerializeField] private Color _mergeableColor = new(0.45f, 1f, 0.45f, 1f);
        [SerializeField] private Color _notMergeableColor = new(1f, 0.4f, 0.4f, 1f);

        public Color MergeableColor => _mergeableColor;
        public Color NotMergeableColor => _notMergeableColor;
    }
}
