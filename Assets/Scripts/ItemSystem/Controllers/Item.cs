using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.GridSystem;
using UnityEngine;

namespace Game.ItemSystem
{
    internal class Item : MonoBehaviour, IGridObject
    {
        [SerializeField] private ItemAnimationConfig _animationConfig;
        [SerializeField] private SpriteRenderer _renderer;

        public void Setup(ItemChainData chain, int level)
        {
            _renderer.sprite = chain.Levels[level - 1].Sprite;
        }

        public UniTask PlaySpawnAsync(Vector2 target, CancellationToken cancellationToken)
        {
            transform.localScale = Vector3.one * _animationConfig.SpawnStartScale;
            return DOTween.Sequence()
                .Join(transform.DOJump(target, _animationConfig.SpawnJumpPower, 1, _animationConfig.SpawnDuration).SetEase(_animationConfig.SpawnEase))
                .Join(transform.DOScale(1f, _animationConfig.SpawnDuration))
                .ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationToken);
        }
    }
}
