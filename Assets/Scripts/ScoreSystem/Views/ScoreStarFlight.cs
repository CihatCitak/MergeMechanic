using DG.Tweening;
using Game.Core.Events;
using Game.Events;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.ScoreSystem
{
    internal class ScoreStarFlight : MonoBehaviour
    {
        [SerializeField] private ScoreConfig _scoreConfig;
        [SerializeField] private Camera _camera;
        [SerializeField] private ScoreView _scoreView;
        [SerializeField] private RectTransform _starPrefab;

        private ObjectPool<RectTransform> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<RectTransform>(
                () => Instantiate(_starPrefab, transform),
                star => star.gameObject.SetActive(true),
                star => star.gameObject.SetActive(false),
                star => Destroy(star.gameObject));
        }

        private void OnEnable()
        {
            EventBus.Subscribe<ItemMergedEvent>(OnItemMerged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ItemMergedEvent>(OnItemMerged);
        }

        private void OnItemMerged(ItemMergedEvent e)
        {
            var star = _pool.Get();
            star.position = _camera.WorldToScreenPoint(e.Position);
            star.DOMove(_scoreView.Icon.position, _scoreConfig.FlightDuration)
                .SetEase(_scoreConfig.FlightEase)
                .SetLink(star.gameObject)
                .OnComplete(() =>
                {
                    _pool.Release(star);
                    _scoreView.Add(e.Score);
                });
        }
    }
}
