using UnityEngine;

namespace Game.GridSystem
{
    [CreateAssetMenu(menuName = "Game/Grid Config", fileName = "GridConfig")]
    internal class GridConfig : ScriptableObject
    {
        [SerializeField] private Vector2Int _size = new(5, 5);
        [SerializeField, Min(0.01f)] private float _cellSize = 1f;
        [SerializeField, Min(0f)] private float _cornerPadding = 0.25f;

        public Vector2Int Size => _size;
        public float CellSize => _cellSize;
        public float CornerPadding => _cornerPadding;
    }
}
