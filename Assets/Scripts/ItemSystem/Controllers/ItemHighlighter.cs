using Game.Core.Services;
using Game.GridSystem;
using UnityEngine;

namespace Game.ItemSystem
{
    internal class ItemHighlighter : MonoBehaviour
    {
        public void Show(Item item, Vector2 world)
        {
            var grid = ServiceLocator.Get<IGrid>();
            var cellHighlighter = ServiceLocator.Get<ICellHighlighter>();
            var cell = grid.WorldToCell(world);
            if (!grid.IsInside(cell) || cell == item.Cell || ServiceLocator.Get<IGridObjectHolder>().Get(cell) is not Item target)
            {
                cellHighlighter.Hide();
                return;
            }

            cellHighlighter.Show(cell, MergeRules.Evaluate(item.Chain, item.Level, target.Chain, target.Level) == MergeResult.Merge);
        }

        public void Hide()
        {
            ServiceLocator.Get<ICellHighlighter>().Hide();
        }
    }
}
