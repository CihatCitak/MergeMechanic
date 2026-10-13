using Game.Core.Events;
using Game.Core.Services;
using Game.Events;
using Game.GridSystem;
using Game.InputSystem;
using UnityEngine;

namespace Game.ItemSystem
{
    internal class Item : MonoBehaviour, IGridObject, IDraggable
    {
        [SerializeField] private ItemView _view;
        [SerializeField] private Collider2D _collider;

        public ItemView View => _view;
        public ItemChainData Chain { get; private set; }
        public int Level { get; private set; }
        public Vector2Int Cell => ServiceLocator.Get<IGridObjectHolder>().CellOf(this);

        public void Setup(ItemChainData chain, int level)
        {
            Chain = chain;
            Level = level;
            _view.SetSprite(chain.GetLevel(level).Sprite);
        }

        public void Place(Vector2Int cell)
        {
            ServiceLocator.Get<IGridObjectHolder>().Occupy(cell, this);
            _collider.enabled = true;
            EventBus.Fire(new ItemPlacedEvent());
        }

        public void Unplace()
        {
            ServiceLocator.Get<IGridObjectHolder>().Vacate(Cell);
            _collider.enabled = false;
        }

        public void Deactivate()
        {
            _view.Stop();
            _collider.enabled = false;
            gameObject.SetActive(false);
        }

        public void BeginDrag()
        {
            _view.BeginDrag();
            ServiceLocator.Get<IItemDragHandler>().Pick(this);
        }

        public void DragTo(Vector2 world)
        {
            _view.Follow(world);
            ServiceLocator.Get<IItemDragHandler>().Drag(this, world);
        }

        public void EndDrag(Vector2 world)
        {
            _view.EndDrag();
            ServiceLocator.Get<IItemDragHandler>().Drop(this, world);
        }
    }
}
