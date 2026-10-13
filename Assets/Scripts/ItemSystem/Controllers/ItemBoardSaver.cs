using System.Collections.Generic;
using Game.Core.Events;
using Game.Core.Services;
using Game.Events;
using Game.GridSystem;
using Game.PlayerSystem;
using UnityEngine;

namespace Game.ItemSystem
{
    internal class ItemBoardSaver : MonoBehaviour
    {
        [SerializeField] private ItemCatalog _itemCatalog;
        [SerializeField] private ItemCreator _creator;

        private readonly List<BoardItemData> _board = new();

        private void Start()
        {
            Restore();
            EventBus.Subscribe<ItemPlacedEvent>(OnItemPlaced);
            EventBus.Subscribe<SceneLoadStartedEvent>(OnSceneLoadStarted);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<ItemPlacedEvent>(OnItemPlaced);
            EventBus.Unsubscribe<SceneLoadStartedEvent>(OnSceneLoadStarted);
        }

        private void Restore()
        {
            var grid = ServiceLocator.Get<IGrid>();
            foreach (var data in ServiceLocator.Get<IPlayerProgress>().Board)
            {
                if (!_itemCatalog.TryGet(data.ChainId, out var chain))
                    continue;
                if (data.Level < 1 || data.Level > chain.Levels.Count || !grid.IsInside(data.Cell))
                    continue;

                _creator.CreatePlaced(chain, data.Level, data.Cell);
            }
        }

        private void OnItemPlaced(ItemPlacedEvent e)
        {
            var size = ServiceLocator.Get<IGrid>().Size;
            var holder = ServiceLocator.Get<IGridObjectHolder>();
            _board.Clear();
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    var cell = new Vector2Int(x, y);
                    if (holder.Get(cell) is Item item)
                        _board.Add(new BoardItemData(item.Chain.Id, item.Level, cell));
                }
            }

            ServiceLocator.Get<IPlayerProgress>().SaveBoard(_board);
        }

        private void OnSceneLoadStarted(SceneLoadStartedEvent e)
        {
            EventBus.Unsubscribe<ItemPlacedEvent>(OnItemPlaced);
        }
    }
}
