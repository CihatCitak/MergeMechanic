using UnityEngine;

namespace Game.Core.Utilities
{
    public static class AngleUtility
    {
        public static float ToDegrees(Vector2Int direction)
        {
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
    }
}
