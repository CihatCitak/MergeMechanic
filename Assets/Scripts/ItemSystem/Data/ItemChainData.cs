using System.Collections.Generic;
using UnityEngine;

namespace Game.ItemSystem
{
    [CreateAssetMenu(menuName = "Game/Item Chain", fileName = "ItemChain")]
    public class ItemChainData : ScriptableObject
    {
        [SerializeField] private ItemLevel[] _levels;

        public IReadOnlyList<ItemLevel> Levels => _levels;
    }
}
