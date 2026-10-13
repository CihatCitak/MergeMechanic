using UnityEngine;

namespace Game.ItemSystem
{
    [CreateAssetMenu(menuName = "Game/Item Catalog", fileName = "ItemCatalog")]
    internal class ItemCatalog : ScriptableObject
    {
        [SerializeField] private ItemChainData[] _chains;

        public bool TryGet(string id, out ItemChainData chain)
        {
            foreach (var candidate in _chains)
            {
                if (candidate.Id == id)
                {
                    chain = candidate;
                    return true;
                }
            }

            chain = null;
            return false;
        }
    }
}
