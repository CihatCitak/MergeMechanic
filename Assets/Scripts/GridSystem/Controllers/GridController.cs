using Game.Core.Services;
using UnityEngine;

namespace Game.GridSystem
{
    internal class GridController : BaseService, IGrid
    {
        [SerializeField] private GridConfig _config;
        [SerializeField] private GridCreator _creator;

        public Rect Area
        {
            get
            {
                var size = GridMath.AreaSize(_config.Size, _config.CellSize, _config.FramePadding);
                return new Rect((Vector2)transform.position - size * 0.5f, size);
            }
        }

        protected override void BindServices()
        {
            ServiceLocator.Bind<IGrid>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IGrid>(this);
        }

        protected override void Initialize()
        {
            _creator.Create(_config);
        }
    }
}
