using Game.Core.Events;
using UnityEngine;

namespace Game.Events
{
    public readonly struct ItemMergedEvent : IEvent
    {
        public readonly Vector2 Position;
        public readonly int Score;

        public ItemMergedEvent(Vector2 position, int score)
        {
            Position = position;
            Score = score;
        }
    }
}
