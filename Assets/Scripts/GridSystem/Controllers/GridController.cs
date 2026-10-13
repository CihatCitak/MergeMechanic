using Game.Core.Services;
using UnityEngine;

namespace Game.GridSystem
{
    internal class GridController : BaseService, IGrid
    {
        [SerializeField] private GridConfig _gridConfig;
        [SerializeField] private GridCreator _creator;

        public Rect Area
        {
            get
            {
                var size = GridMath.AreaSize(_gridConfig.Size, _gridConfig.CellSize, _gridConfig.CornerPadding);
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
            _creator.Create(_gridConfig);
        }

        public Vector2 CellToWorld(Vector2Int cell)
        {
            return (Vector2)transform.position + GridMath.CellToLocal(cell, _gridConfig.Size, _gridConfig.CellSize);
        }
    }
}
