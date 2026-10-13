using UnityEngine;

namespace Game.InputSystem
{
    public interface IDraggable
    {
        void BeginDrag();
        void DragTo(Vector2 world);
        void EndDrag(Vector2 world);
    }
}
