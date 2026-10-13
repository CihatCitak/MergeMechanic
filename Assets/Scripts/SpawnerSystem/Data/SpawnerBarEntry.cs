using System;
using UnityEngine;

namespace Game.SpawnerSystem
{
    [Serializable]
    internal class SpawnerBarEntry
    {
        [SerializeField] private SpawnerData _spawner;
        [SerializeField, Min(1)] private int _spawnLevel = 1;

        public SpawnerData Spawner => _spawner;
        public int SpawnLevel => _spawnLevel;
    }
}
