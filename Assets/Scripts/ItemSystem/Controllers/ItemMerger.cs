using Game.Core.Services;
using Game.GridSystem;
using UnityEngine;

namespace Game.ItemSystem
{
    internal class ItemMerger : MonoBehaviour
    {
        [SerializeField] private ItemCreator _creator;

        public void Merge(Item dragged, Item target)
        {
            var cell = target.Cell;
            var chain = dragged.Chain;
            var mergedLevel = dragged.Level + 1;
            dragged.Unplace();
            target.Unplace();
            ServiceLocator.Get<IGridObjectHolder>().Reserve(cell);
            _creator.Recycle(dragged);
            _creator.Recycle(target);
            _creator.CreateMerged(chain, mergedLevel, cell);
        }
    }
}
