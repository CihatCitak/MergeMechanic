using Cysharp.Threading.Tasks;
using Game.Core.Services;
using Game.GridSystem;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.ItemSystem
{
    internal class ItemCreator : BaseService, IItemCreator
    {
        [SerializeField] private Item _prefab;

        private ObjectPool<Item> _pool;

        protected override void BindServices()
        {
            ServiceLocator.Bind<IItemCreator>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IItemCreator>(this);
        }

        protected override void Awake()
        {
            base.Awake();
            _pool = new ObjectPool<Item>(
                () => Instantiate(_prefab, transform),
                item => item.gameObject.SetActive(true),
                item => item.Deactivate(),
                item => Destroy(item.gameObject));
        }

        public void Create(ItemChainData chain, int level, Vector2 origin, Vector2Int cell)
        {
            SpawnAsync(chain, level, origin, cell).Forget();
        }

        public Item CreateMerged(ItemChainData chain, int level, Vector2Int cell)
        {
            var item = CreatePlaced(chain, level, cell);
            item.PlayMerge();
            return item;
        }

        public Item CreatePlaced(ItemChainData chain, int level, Vector2Int cell)
        {
            var item = CreateItem(chain, level, ServiceLocator.Get<IGrid>().CellToWorld(cell));
            ServiceLocator.Get<IGridObjectHolder>().Reserve(cell);
            item.Place(cell);
            return item;
        }

        public void Recycle(Item item)
        {
            _pool.Release(item);
        }

        private async UniTaskVoid SpawnAsync(ItemChainData chain, int level, Vector2 origin, Vector2Int cell)
        {
            var item = CreateItem(chain, level, origin);
            await item.PlaySpawnAsync(ServiceLocator.Get<IGrid>().CellToWorld(cell), item.destroyCancellationToken);
            item.Place(cell);
        }

        private Item CreateItem(ItemChainData chain, int level, Vector2 position)
        {
            var item = _pool.Get();
            item.transform.position = position;
            item.Setup(chain, level);
            return item;
        }
    }
}
