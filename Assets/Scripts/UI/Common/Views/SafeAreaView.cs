using UnityEngine;

namespace Game.UI.Common
{
    internal class SafeAreaView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;

        private void Awake()
        {
            var area = Screen.safeArea;
            _rect.anchorMin = new Vector2(area.xMin / Screen.width, area.yMin / Screen.height);
            _rect.anchorMax = new Vector2(area.xMax / Screen.width, area.yMax / Screen.height);
        }
    }
}
