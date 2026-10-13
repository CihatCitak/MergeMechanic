using System.Collections.Generic;
using Game.Core.Services;
using Game.GridSystem;
using UnityEngine;

namespace Game.ItemSystem
{
    internal class ItemHighlighter : MonoBehaviour
    {
        private readonly List<Vector2Int> _candidates = new();

        public void ShowCandidates(Item item)
        {
            var size = ServiceLocator.Get<IGrid>().Size;
            var holder = ServiceLocator.Get<IGridObjectHolder>();
            _candidates.Clear();
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    var cell = new Vector2Int(x, y);
                    if (holder.Get(cell) is Item other && other != item && CanMerge(item, other))
                        _candidates.Add(cell);
                }
            }

            ServiceLocator.Get<ICellHighlighter>().ShowCandidates(_candidates);
        }

        public void ShowTarget(Item item, Vector2 world)
        {
            var grid = ServiceLocator.Get<IGrid>();
            var cellHighlighter = ServiceLocator.Get<ICellHighlighter>();
            var cell = grid.WorldToCell(world);
            if (!grid.IsInside(cell) || cell == item.Cell || ServiceLocator.Get<IGridObjectHolder>().Get(cell) is not Item target)
            {
                cellHighlighter.HideTarget();
                return;
            }

            cellHighlighter.ShowTarget(cell, CanMerge(item, target));
        }

        public void Hide()
        {
            var cellHighlighter = ServiceLocator.Get<ICellHighlighter>();
            cellHighlighter.HideTarget();
            cellHighlighter.HideCandidates();
        }

        private static bool CanMerge(Item item, Item other)
        {
            return MergeRules.Evaluate(item.Chain, item.Level, other.Chain, other.Level) == MergeResult.Merge;
        }
    }
}
