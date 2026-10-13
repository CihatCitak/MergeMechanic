using UnityEngine;

namespace Game.ItemSystem
{
    public interface IItemCreator
    {
        void Create(ItemChainData chain, int level, Vector2 origin, Vector2Int cell);
    }
}
