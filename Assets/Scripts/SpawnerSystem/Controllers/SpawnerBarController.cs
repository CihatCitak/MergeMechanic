using Game.Core.Services;
using Game.GridSystem;
using UnityEngine;

namespace Game.SpawnerSystem
{
    internal class SpawnerBarController : MonoBehaviour
    {
        [SerializeField] private SpawnerBarConfig _spawnerBarConfig;
        [SerializeField] private SpawnerBarCreator _creator;

        private void Start()
        {
            var gridArea = ServiceLocator.Get<IGrid>().Area;
            var firstX = gridArea.center.x - (_spawnerBarConfig.Spawners.Count - 1) * _spawnerBarConfig.Spacing * 0.5f;
            var y = gridArea.yMin - _spawnerBarConfig.GridGap - _spawnerBarConfig.SpawnerSize * 0.5f;
            for (var i = 0; i < _spawnerBarConfig.Spawners.Count; i++)
            {
                var entry = _spawnerBarConfig.Spawners[i];
                _creator.Create(entry.Spawner, entry.SpawnLevel, new Vector2(firstX + i * _spawnerBarConfig.Spacing, y), _spawnerBarConfig.SpawnerSize);
            }
        }
    }
}
