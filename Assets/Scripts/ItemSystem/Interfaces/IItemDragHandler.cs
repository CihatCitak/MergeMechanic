using UnityEngine;

namespace Game.ItemSystem
{
    internal interface IItemDragHandler
    {
        void Pick(Item item);
        void Drag(Item item, Vector2 world);
        void Drop(Item item, Vector2 world);
    }
}
