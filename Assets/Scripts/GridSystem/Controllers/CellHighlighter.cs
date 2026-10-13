using System.Collections.Generic;
using Game.Core.Services;
using UnityEngine;

namespace Game.GridSystem
{
    internal class CellHighlighter : BaseService, ICellHighlighter
    {
        [SerializeField] private GridConfig _gridConfig;
        [SerializeField] private CellHighlighterConfig _cellHighlighterConfig;
        [SerializeField] private SpriteRenderer _highlight;
        [SerializeField] private SpriteRenderer _candidatePrefab;

        private readonly List<SpriteRenderer> _candidates = new();

        protected override void BindServices()
        {
            ServiceLocator.Bind<ICellHighlighter>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<ICellHighlighter>(this);
        }

        public void ShowTarget(Vector2Int cell, bool isMergeable)
        {
            PlaceOnCell(_highlight, cell);
            _highlight.color = isMergeable ? _cellHighlighterConfig.MergeableColor : _cellHighlighterConfig.NotMergeableColor;
            _highlight.gameObject.SetActive(true);
        }

        public void HideTarget()
        {
            _highlight.gameObject.SetActive(false);
        }

        public void ShowCandidates(IReadOnlyList<Vector2Int> cells)
        {
            while (_candidates.Count < cells.Count)
                _candidates.Add(Instantiate(_candidatePrefab, transform));

            for (var i = 0; i < _candidates.Count; i++)
            {
                var isUsed = i < cells.Count;
                if (isUsed)
                    PlaceOnCell(_candidates[i], cells[i]);
                _candidates[i].gameObject.SetActive(isUsed);
            }
        }

        public void HideCandidates()
        {
            foreach (var candidate in _candidates)
                candidate.gameObject.SetActive(false);
        }

        private void PlaceOnCell(SpriteRenderer highlight, Vector2Int cell)
        {
            highlight.transform.position = ServiceLocator.Get<IGrid>().CellToWorld(cell);
            highlight.transform.localScale = Vector3.one * _gridConfig.CellSize;
        }
    }
}
