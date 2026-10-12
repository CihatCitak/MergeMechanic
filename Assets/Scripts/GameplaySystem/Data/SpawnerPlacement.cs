using System;
using Game.SpawnerSystem;
using UnityEngine;

namespace Game.GameplaySystem
{
    [Serializable]
    internal class SpawnerPlacement
    {
        [SerializeField] private SpawnerData _spawner;
        [SerializeField] private Vector2Int _cell;

        public SpawnerData Spawner => _spawner;
        public Vector2Int Cell => _cell;
    }
}
