using Cysharp.Threading.Tasks;
using Game.Core.Services;
using Game.GridSystem;
using Game.UI.Toast;
using UnityEngine;

namespace Game.ItemSystem
{
    internal class ItemDragHandler : BaseService, IItemDragHandler
    {
        [SerializeField] private ItemDragConfig _itemDragConfig;
        [SerializeField] private ItemHighlighter _highlighter;
        [SerializeField] private ItemMerger _merger;

        protected override void BindServices()
        {
            ServiceLocator.Bind<IItemDragHandler>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IItemDragHandler>(this);
        }

        public void Pick(Item item)
        {
            _highlighter.ShowCandidates(item);
        }

        public void Drag(Item item, Vector2 world)
        {
            var cell = ServiceLocator.Get<IGrid>().WorldToCell(world);
            if (Locate(item, cell, out var target) != DropCell.Item)
            {
                _highlighter.HideTarget();
                return;
            }

            _highlighter.ShowTarget(cell, MergeRules.Evaluate(item.Chain, item.Level, target.Chain, target.Level) == MergeResult.Merge);
        }

        public void Drop(Item item, Vector2 world)
        {
            _highlighter.Hide();
            var grid = ServiceLocator.Get<IGrid>();
            var cell = grid.WorldToCell(world);
            switch (Locate(item, cell, out var target))
            {
                case DropCell.Empty:
                    MoveAsync(item, cell).Forget();
                    break;
                case DropCell.Item:
                    DropOn(item, target);
                    break;
                default:
                    item.View.MoveToAsync(grid.CellToWorld(item.Cell), item.destroyCancellationToken).Forget();
                    break;
            }
        }

        private void DropOn(Item item, Item target)
        {
            var result = MergeRules.Evaluate(item.Chain, item.Level, target.Chain, target.Level);
            if (result == MergeResult.Merge)
            {
                _merger.Merge(item, target);
                return;
            }

            if (result == MergeResult.MaxLevel)
                ServiceLocator.Get<IToastService>().Show(_itemDragConfig.MaxLevelMessage);

            RejectAsync(item).Forget();
        }

        private static DropCell Locate(Item item, Vector2Int cell, out Item target)
        {
            target = null;
            var holder = ServiceLocator.Get<IGridObjectHolder>();
            if (!ServiceLocator.Get<IGrid>().IsInside(cell) || cell == item.Cell)
                return DropCell.Unavailable;
            if (holder.IsEmpty(cell))
                return DropCell.Empty;

            target = holder.Get(cell) as Item;
            return target != null ? DropCell.Item : DropCell.Unavailable;
        }

        private static async UniTaskVoid MoveAsync(Item item, Vector2Int cell)
        {
            item.Unplace();
            ServiceLocator.Get<IGridObjectHolder>().Reserve(cell);
            await item.View.MoveToAsync(ServiceLocator.Get<IGrid>().CellToWorld(cell), item.destroyCancellationToken);
            item.Place(cell);
        }

        private static async UniTaskVoid RejectAsync(Item item)
        {
            await item.View.MoveToAsync(ServiceLocator.Get<IGrid>().CellToWorld(item.Cell), item.destroyCancellationToken);
            item.View.PlayReject();
        }
    }
}
