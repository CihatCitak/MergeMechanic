using UnityEngine;

namespace Game.ItemSystem
{
    [CreateAssetMenu(menuName = "Game/Item Drag Config", fileName = "ItemDragConfig")]
    internal class ItemDragConfig : ScriptableObject
    {
        [SerializeField] private int _dragSortingOrder = 20;
        [SerializeField] private string _maxLevelMessage = "Maksimum seviye!";

        public int DragSortingOrder => _dragSortingOrder;
        public string MaxLevelMessage => _maxLevelMessage;
    }
}
