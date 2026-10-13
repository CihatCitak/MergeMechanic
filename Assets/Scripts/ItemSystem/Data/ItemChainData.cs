using System.Collections.Generic;
using UnityEngine;

namespace Game.ItemSystem
{
    [CreateAssetMenu(menuName = "Game/Item Chain", fileName = "ItemChain")]
    public class ItemChainData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private ItemLevel[] _levels;

        public string Id => _id;
        public IReadOnlyList<ItemLevel> Levels => _levels;

        public ItemLevel GetLevel(int level) => _levels[level - 1];
    }
}
