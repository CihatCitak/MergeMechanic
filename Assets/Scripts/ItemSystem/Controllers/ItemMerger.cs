using Game.Core.Events;
using Game.Events;
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
            _creator.Recycle(dragged);
            _creator.Recycle(target);
            var merged = _creator.CreateMerged(chain, mergedLevel, cell);
            EventBus.Fire(new ItemMergedEvent(merged.transform.position, chain.GetLevel(mergedLevel).MergeScore));
        }
    }
}
