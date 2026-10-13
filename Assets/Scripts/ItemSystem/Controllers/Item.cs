using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        [SerializeField] private ItemAnimationConfig _animationConfig;
        [SerializeField] private ItemDragConfig _itemDragConfig;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Collider2D _collider;

        private int _sortingOrder;
        private Tween _moveTween;

        public ItemChainData Chain { get; private set; }
        public int Level { get; private set; }
        public Vector2Int Cell { get; private set; }

        private void Awake()
        {
            _sortingOrder = _renderer.sortingOrder;
        }

        public void Setup(ItemChainData chain, int level)
        {
            Chain = chain;
            Level = level;
            _renderer.sprite = chain.GetLevel(level).Sprite;
        }

        public void Place(Vector2Int cell)
        {
            ServiceLocator.Get<IGridObjectHolder>().Occupy(cell, this);
            Cell = cell;
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
            transform.DOKill();
            _renderer.transform.DOKill(true);
            _collider.enabled = false;
            _renderer.sortingOrder = _sortingOrder;
            gameObject.SetActive(false);
        }

        public void BeginDrag()
        {
            if (_moveTween.IsActive())
                _moveTween.Kill();
            _renderer.sortingOrder = _itemDragConfig.DragSortingOrder;
            ServiceLocator.Get<IItemDragHandler>().Pick(this);
        }

        public void DragTo(Vector2 world)
        {
            transform.position = world;
            ServiceLocator.Get<IItemDragHandler>().Drag(this, world);
        }

        public void EndDrag(Vector2 world)
        {
            _renderer.sortingOrder = _sortingOrder;
            ServiceLocator.Get<IItemDragHandler>().Drop(this, world);
        }

        public UniTask PlaySpawnAsync(Vector2 target, CancellationToken cancellationToken)
        {
            _renderer.transform.localScale = Vector3.one * _animationConfig.SpawnStartScale;
            return DOTween.Sequence()
                .Join(transform.DOJump(target, _animationConfig.SpawnJumpPower, 1, _animationConfig.SpawnDuration).SetEase(_animationConfig.SpawnEase))
                .Join(_renderer.transform.DOScale(1f, _animationConfig.SpawnDuration))
                .ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationToken);
        }

        public void PlayMerge()
        {
            _renderer.transform.localScale = Vector3.one * _animationConfig.MergeStartScale;
            _renderer.transform.DOScale(1f, _animationConfig.MergeDuration).SetEase(_animationConfig.MergeEase).SetLink(gameObject);
        }

        public void PlayReject()
        {
            _renderer.transform.DOKill(true);
            _renderer.transform.DOShakePosition(_animationConfig.RejectDuration, Vector3.right * _animationConfig.RejectStrength, _animationConfig.RejectVibrato, 0f)
                .SetLink(gameObject);
        }

        public UniTask MoveToAsync(Vector2 target, CancellationToken cancellationToken)
        {
            _moveTween = transform.DOMove(target, _animationConfig.MoveDuration).SetEase(_animationConfig.MoveEase);
            return _moveTween.ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationToken);
        }
    }
}
