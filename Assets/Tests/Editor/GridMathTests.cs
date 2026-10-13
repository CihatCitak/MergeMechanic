using Game.GridSystem;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests
{
    public class GridMathTests
    {
        private static readonly Vector2Int Size = new(5, 5);

        [Test]
        public void CellToLocal_EvenSize_CentersGrid()
        {
            var size = new Vector2Int(4, 2);

            Assert.AreEqual(new Vector2(-3f, -1f), GridMath.CellToLocal(Vector2Int.zero, size, 2f));
            Assert.AreEqual(new Vector2(3f, 1f), GridMath.CellToLocal(new Vector2Int(3, 1), size, 2f));
        }

        [Test]
        public void LocalToCell_PointNearCellEdge_StaysInCell()
        {
            Assert.AreEqual(new Vector2Int(2, 2), GridMath.LocalToCell(new Vector2(0.49f, -0.49f), Size, 1f));
            Assert.AreEqual(new Vector2Int(3, 1), GridMath.LocalToCell(new Vector2(0.51f, -0.51f), Size, 1f));
        }

        [Test]
        public void LocalToCell_PointOutsideBoard_IsOutsideGrid()
        {
            Assert.IsFalse(GridMath.IsInside(GridMath.LocalToCell(new Vector2(2.6f, 0f), Size, 1f), Size));
            Assert.IsFalse(GridMath.IsInside(GridMath.LocalToCell(new Vector2(0f, -2.6f), Size, 1f), Size));
        }
    }
}
