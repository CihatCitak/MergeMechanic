using UnityEngine;

namespace Game.ItemSystem
{
    internal interface IItemDragHandler
    {
        void Drag(Item item, Vector2 world);
        void Drop(Item item, Vector2 world);
    }
}
