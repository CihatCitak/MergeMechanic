using System.Collections.Generic;
using UnityEngine;

namespace Game.SpawnerSystem
{
    [CreateAssetMenu(menuName = "Game/Spawner", fileName = "Spawner")]
    public class SpawnerData : ScriptableObject
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private SpawnerChainWeight[] _chains;

        public Sprite Sprite => _sprite;
        public IReadOnlyList<SpawnerChainWeight> Chains => _chains;
    }
}
