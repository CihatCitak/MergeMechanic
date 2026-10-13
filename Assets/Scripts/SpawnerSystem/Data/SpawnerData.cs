using Game.ItemSystem;
using UnityEngine;

namespace Game.SpawnerSystem
{
    [CreateAssetMenu(menuName = "Game/Spawner", fileName = "Spawner")]
    public class SpawnerData : ScriptableObject
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private ItemChainData _chain;

        public Sprite Sprite => _sprite;
        public ItemChainData Chain => _chain;
    }
}
