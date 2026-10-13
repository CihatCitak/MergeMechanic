using Game.Core.Services;
using Game.GridSystem;
using UnityEngine;

namespace Game.CameraSystem
{
    internal class CameraFitter : MonoBehaviour
    {
        [SerializeField] private CameraConfig _config;
        [SerializeField] private Camera _camera;

        private void Start()
        {
            Fit(ServiceLocator.Get<IGrid>().Area);
        }

        private void Fit(Rect area)
        {
            var halfHeight = area.height * 0.5f + _config.Padding;
            var halfWidth = area.width * 0.5f + _config.Padding;
            _camera.orthographicSize = Mathf.Max(halfHeight, halfWidth / _camera.aspect);
            _camera.transform.position = new Vector3(area.center.x, area.center.y, _camera.transform.position.z);
        }
    }
}
