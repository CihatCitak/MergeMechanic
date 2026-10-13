using System;
using System.Collections.Generic;

namespace Game.PlayerSystem
{
    [Serializable]
    internal class PlayerData
    {
        public int Score;
        public List<BoardItemData> Board = new();
    }
}
