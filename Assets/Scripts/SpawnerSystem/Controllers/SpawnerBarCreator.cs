using UnityEngine;

namespace Game.SpawnerSystem
{
    internal class SpawnerBarCreator : MonoBehaviour
    {
        [SerializeField] private Spawner _prefab;

        public void Create(SpawnerData data, int spawnLevel, Vector2 position, float size)
        {
            var spawner = Instantiate(_prefab, position, Quaternion.identity, transform);
            spawner.transform.localScale = Vector3.one * size;
            spawner.Setup(data, spawnLevel);
        }
    }
}
