using UnityEngine;

namespace Game.GridSystem
{
    [CreateAssetMenu(menuName = "Game/Cell Highlighter Config", fileName = "CellHighlighterConfig")]
    internal class CellHighlighterConfig : ScriptableObject
    {
        [SerializeField] private Color _validColor = new(0.45f, 1f, 0.45f, 1f);
        [SerializeField] private Color _invalidColor = new(1f, 0.4f, 0.4f, 1f);

        public Color ValidColor => _validColor;
        public Color InvalidColor => _invalidColor;
    }
}
