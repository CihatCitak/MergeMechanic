using Game.GridSystem;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests
{
    public class GridMathTests
    {
        [Test]
        public void CellToLocal_OddSize_CenterCellIsOrigin()
        {
            Assert.AreEqual(Vector2.zero, GridMath.CellToLocal(new Vector2Int(2, 2), new Vector2Int(5, 5), 1f));
        }

        [Test]
        public void CellToLocal_EvenSize_CentersGrid()
        {
            var size = new Vector2Int(4, 2);

            Assert.AreEqual(new Vector2(-3f, -1f), GridMath.CellToLocal(Vector2Int.zero, size, 2f));
            Assert.AreEqual(new Vector2(3f, 1f), GridMath.CellToLocal(new Vector2Int(3, 1), size, 2f));
        }
    }
}
