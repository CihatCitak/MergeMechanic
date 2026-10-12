using System;
using UnityEngine;

namespace Game.ItemSystem
{
    [Serializable]
    public class ItemLevel
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField, Min(0)] private int _mergeScore;

        public Sprite Sprite => _sprite;
        public int MergeScore => _mergeScore;
    }
}
