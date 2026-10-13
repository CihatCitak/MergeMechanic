using UnityEngine;

namespace Game.GridSystem
{
    internal class GridCreator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _cellPrefab;
        [SerializeField] private SpriteRenderer _frame;

        public void Create(GridConfig config)
        {
            _frame.size = GridMath.AreaSize(config.Size, config.CellSize, config.FramePadding);
            for (var x = 0; x < config.Size.x; x++)
            {
                for (var y = 0; y < config.Size.y; y++)
                {
                    var cell = Instantiate(_cellPrefab, transform);
                    cell.transform.localPosition = GridMath.CellToLocal(new Vector2Int(x, y), config.Size, config.CellSize);
                    cell.transform.localScale = Vector3.one * config.CellSize;
                }
            }
        }
    }
}
