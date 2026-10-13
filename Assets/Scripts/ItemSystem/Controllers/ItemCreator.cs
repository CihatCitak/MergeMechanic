using Cysharp.Threading.Tasks;
using Game.Core.Services;
using Game.GridSystem;
using UnityEngine;

namespace Game.ItemSystem
{
    internal class ItemCreator : BaseService, IItemCreator
    {
        [SerializeField] private Item _prefab;

        protected override void BindServices()
        {
            ServiceLocator.Bind<IItemCreator>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IItemCreator>(this);
        }

        public void Create(ItemChainData chain, int level, Vector2 origin, Vector2Int cell)
        {
            CreateAsync(chain, level, origin, cell).Forget();
        }

        private async UniTaskVoid CreateAsync(ItemChainData chain, int level, Vector2 origin, Vector2Int cell)
        {
            var item = Instantiate(_prefab, origin, Quaternion.identity, transform);
            item.Setup(chain, level);
            await item.PlaySpawnAsync(ServiceLocator.Get<IGrid>().CellToWorld(cell), item.destroyCancellationToken);
            ServiceLocator.Get<IGridObjectHolder>().Occupy(cell, item);
        }
    }
}
