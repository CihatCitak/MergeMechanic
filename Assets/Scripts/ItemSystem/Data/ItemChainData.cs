using System.Collections.Generic;
using UnityEngine;

namespace Game.ItemSystem
{
    [CreateAssetMenu(menuName = "Game/Item Chain", fileName = "ItemChain")]
    public class ItemChainData : ScriptableObject
    {
        [SerializeField] private ItemLevel[] _levels;

        public IReadOnlyList<ItemLevel> Levels => _levels;

        public ItemLevel GetLevel(int level) => _levels[level - 1];
    }
}
