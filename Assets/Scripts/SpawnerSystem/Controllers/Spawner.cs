using DG.Tweening;
using Game.Core.Services;
using Game.GridSystem;
using Game.InputSystem;
using Game.ItemSystem;
using Game.UI.Toast;
using UnityEngine;

namespace Game.SpawnerSystem
{
    internal class Spawner : MonoBehaviour, ITappable
    {
        [SerializeField] private SpawnerConfig _spawnerConfig;
        [SerializeField] private SpriteRenderer _renderer;

        private SpawnerData _data;
        private int _spawnLevel;

        public void Setup(SpawnerData data, int spawnLevel)
        {
            _data = data;
            _spawnLevel = spawnLevel;
            _renderer.sprite = data.Sprite;
        }

        public void Tap()
        {
            PlayTapFeedback();
            if (!ServiceLocator.Get<IGridObjectHolder>().TryReserveRandomEmptyCell(out var cell))
            {
                ServiceLocator.Get<IToastService>().Show(_spawnerConfig.GridFullMessage);
                return;
            }

            var bounds = _renderer.bounds;
            ServiceLocator.Get<IItemCreator>().Create(_data.Chain, _spawnLevel, new Vector2(bounds.center.x, bounds.max.y), cell);
        }

        private void PlayTapFeedback()
        {
            transform.DOKill(true);
            transform.DOPunchScale(transform.localScale * _spawnerConfig.TapPunch, _spawnerConfig.TapPunchDuration, 1, 0f).SetLink(gameObject);
        }
    }
}
