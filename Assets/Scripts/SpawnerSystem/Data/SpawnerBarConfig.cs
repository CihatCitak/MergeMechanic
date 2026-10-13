using System.Collections.Generic;
using UnityEngine;

namespace Game.SpawnerSystem
{
    [CreateAssetMenu(menuName = "Game/Spawner Bar Config", fileName = "SpawnerBarConfig")]
    internal class SpawnerBarConfig : ScriptableObject
    {
        [SerializeField] private SpawnerBarEntry[] _spawners;
        [SerializeField, Min(0.01f)] private float _spawnerSize = 1.8f;
        [SerializeField, Min(0f)] private float _spacing = 2f;
        [SerializeField, Min(0f)] private float _gridGap = 0.4f;

        public IReadOnlyList<SpawnerBarEntry> Spawners => _spawners;
        public float SpawnerSize => _spawnerSize;
        public float Spacing => _spacing;
        public float GridGap => _gridGap;
    }
}
