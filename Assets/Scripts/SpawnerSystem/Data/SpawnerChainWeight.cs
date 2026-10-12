using System;
using Game.ItemSystem;
using UnityEngine;

namespace Game.SpawnerSystem
{
    [Serializable]
    public class SpawnerChainWeight
    {
        [SerializeField] private ItemChainData _chain;
        [SerializeField, Min(1)] private int _weight = 1;

        public ItemChainData Chain => _chain;
        public int Weight => _weight;
    }
}
