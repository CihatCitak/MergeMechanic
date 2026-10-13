using DG.Tweening;
using UnityEngine;

namespace Game.ItemSystem
{
    [CreateAssetMenu(menuName = "Game/Item Animation Config", fileName = "ItemAnimationConfig")]
    internal class ItemAnimationConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _spawnDuration = 0.45f;
        [SerializeField, Min(0f)] private float _spawnJumpPower = 1f;
        [SerializeField] private Ease _spawnEase = Ease.OutQuad;
        [SerializeField, Range(0f, 1f)] private float _spawnStartScale = 0.7f;
        [SerializeField, Min(0f)] private float _moveDuration = 0.15f;
        [SerializeField] private Ease _moveEase = Ease.OutQuad;
        [SerializeField, Min(0f)] private float _mergeDuration = 0.3f;
        [SerializeField] private Ease _mergeEase = Ease.OutBack;
        [SerializeField, Range(0f, 1f)] private float _mergeStartScale = 0.5f;

        public float SpawnDuration => _spawnDuration;
        public float SpawnJumpPower => _spawnJumpPower;
        public Ease SpawnEase => _spawnEase;
        public float SpawnStartScale => _spawnStartScale;
        public float MoveDuration => _moveDuration;
        public Ease MoveEase => _moveEase;
        public float MergeDuration => _mergeDuration;
        public Ease MergeEase => _mergeEase;
        public float MergeStartScale => _mergeStartScale;
    }
}
