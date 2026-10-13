using UnityEngine;

namespace Game.GridSystem
{
    internal class GridCorner : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;

        public void SetSize(Vector2 size)
        {
            _renderer.size = size;
        }
    }
}
