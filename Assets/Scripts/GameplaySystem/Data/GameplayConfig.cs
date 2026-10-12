using System.Collections.Generic;
using UnityEngine;

namespace Game.GameplaySystem
{
    [CreateAssetMenu(menuName = "Game/Gameplay Config", fileName = "GameplayConfig")]
    internal class GameplayConfig : ScriptableObject
    {
        [SerializeField] private Vector2Int _gridSize = new(5, 5);
        [SerializeField] private SpawnerPlacement[] _spawners;

        public Vector2Int GridSize => _gridSize;
        public IReadOnlyList<SpawnerPlacement> Spawners => _spawners;
    }
}
