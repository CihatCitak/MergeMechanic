using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game.ItemSystem
{
    internal class ItemView : MonoBehaviour
    {
        [SerializeField] private ItemAnimationConfig _animationConfig;
        [SerializeField] private ItemDragConfig _itemDragConfig;
        [SerializeField] private SpriteRenderer _renderer;

        private int _sortingOrder;
        private Tween _moveTween;

        private Transform Visual => _renderer.transform;

        private void Awake()
        {
            _sortingOrder = _renderer.sortingOrder;
        }

        public void SetSprite(Sprite sprite)
        {
            _renderer.sprite = sprite;
        }

        public void BeginDrag()
        {
            if (_moveTween.IsActive())
                _moveTween.Kill();
            _renderer.sortingOrder = _itemDragConfig.DragSortingOrder;
        }

        public void Follow(Vector2 world)
        {
            transform.position = world;
        }

        public void EndDrag()
        {
            _renderer.sortingOrder = _sortingOrder;
        }

        public UniTask PlaySpawnAsync(Vector2 target, CancellationToken cancellationToken)
        {
            Visual.localScale = Vector3.one * _animationConfig.SpawnStartScale;
            return DOTween.Sequence()
                .Join(transform.DOJump(target, _animationConfig.SpawnJumpPower, 1, _animationConfig.SpawnDuration).SetEase(_animationConfig.SpawnEase))
                .Join(Visual.DOScale(1f, _animationConfig.SpawnDuration))
                .ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationToken);
        }

        public void PlayMerge()
        {
            Visual.localScale = Vector3.one * _animationConfig.MergeStartScale;
            Visual.DOScale(1f, _animationConfig.MergeDuration).SetEase(_animationConfig.MergeEase).SetLink(gameObject);
        }

        public void PlayReject()
        {
            Visual.DOKill(true);
            Visual.DOShakePosition(_animationConfig.RejectDuration, Vector3.right * _animationConfig.RejectStrength, _animationConfig.RejectVibrato, 0f)
                .SetLink(gameObject);
        }

        public UniTask MoveToAsync(Vector2 target, CancellationToken cancellationToken)
        {
            _moveTween = transform.DOMove(target, _animationConfig.MoveDuration).SetEase(_animationConfig.MoveEase);
            return _moveTween.ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationToken);
        }

        public void Stop()
        {
            transform.DOKill();
            Visual.DOKill(true);
            _renderer.sortingOrder = _sortingOrder;
        }
    }
}
