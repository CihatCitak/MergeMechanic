using System;
using UnityEngine;

namespace Game.PlayerSystem
{
    [Serializable]
    public struct BoardItemData
    {
        [SerializeField] private string _chainId;
        [SerializeField] private int _level;
        [SerializeField] private Vector2Int _cell;

        public string ChainId => _chainId;
        public int Level => _level;
        public Vector2Int Cell => _cell;

        public BoardItemData(string chainId, int level, Vector2Int cell)
        {
            _chainId = chainId;
            _level = level;
            _cell = cell;
        }
    }
}
