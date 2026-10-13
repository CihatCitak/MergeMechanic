using UnityEngine;

namespace Game.GridSystem
{
    internal class GridCreator : MonoBehaviour
    {
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private GridCorner _cornerPrefab;

        public void Create(GridConfig gridConfig)
        {
            Instantiate(_cornerPrefab, transform).SetSize(GridMath.AreaSize(gridConfig.Size, gridConfig.CellSize, gridConfig.CornerPadding));
            for (var x = 0; x < gridConfig.Size.x; x++)
            {
                for (var y = 0; y < gridConfig.Size.y; y++)
                {
                    var cell = Instantiate(_cellPrefab, transform);
                    cell.transform.localPosition = GridMath.CellToLocal(new Vector2Int(x, y), gridConfig.Size, gridConfig.CellSize);
                    cell.transform.localScale = Vector3.one * gridConfig.CellSize;
                }
            }
        }
    }
}
