using Game.Core.Services;
using UnityEngine;

namespace Game.GridSystem
{
    internal class CellHighlighter : BaseService, ICellHighlighter
    {
        [SerializeField] private GridConfig _gridConfig;
        [SerializeField] private CellHighlighterConfig _cellHighlighterConfig;
        [SerializeField] private SpriteRenderer _highlight;

        protected override void BindServices()
        {
            ServiceLocator.Bind<ICellHighlighter>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<ICellHighlighter>(this);
        }

        public void Show(Vector2Int cell, bool isMergeable)
        {
            _highlight.transform.position = ServiceLocator.Get<IGrid>().CellToWorld(cell);
            _highlight.transform.localScale = Vector3.one * _gridConfig.CellSize;
            _highlight.color = isMergeable ? _cellHighlighterConfig.MergeableColor : _cellHighlighterConfig.NotMergeableColor;
            _highlight.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _highlight.gameObject.SetActive(false);
        }
    }
}
