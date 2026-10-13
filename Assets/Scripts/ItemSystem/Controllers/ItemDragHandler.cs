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

        public void Drag(Item item, Vector2 world)
        {
            _highlighter.Show(item, world);
        }

        public void Drop(Item item, Vector2 world)
        {
            _highlighter.Hide();
            var grid = ServiceLocator.Get<IGrid>();
            var holder = ServiceLocator.Get<IGridObjectHolder>();
            var cell = grid.WorldToCell(world);
            if (grid.IsInside(cell) && cell != item.Cell)
            {
                if (holder.IsEmpty(cell))
                {
                    MoveAsync(item, cell).Forget();
                    return;
                }

                if (holder.Get(cell) is Item target)
                {
                    var result = MergeRules.Evaluate(item.Chain, item.Level, target.Chain, target.Level);
                    if (result == MergeResult.Merge)
                    {
                        _merger.Merge(item, target);
                        return;
                    }

                    if (result == MergeResult.MaxLevel)
                        ServiceLocator.Get<IToastService>().Show(_itemDragConfig.MaxLevelMessage);
                }
            }

            item.MoveToAsync(grid.CellToWorld(item.Cell), item.destroyCancellationToken).Forget();
        }

        private static async UniTaskVoid MoveAsync(Item item, Vector2Int cell)
        {
            item.Unplace();
            ServiceLocator.Get<IGridObjectHolder>().Reserve(cell);
            await item.MoveToAsync(ServiceLocator.Get<IGrid>().CellToWorld(cell), item.destroyCancellationToken);
            item.Place(cell);
        }
    }
}
