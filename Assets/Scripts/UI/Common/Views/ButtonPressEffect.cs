using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Common
{
    internal class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private UIConfig _uiConfig;
        [SerializeField] private Selectable _target;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_target.IsInteractable())
                ScaleTo(_uiConfig.PressScale);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ScaleTo(1f);
        }

        private void ScaleTo(float scale)
        {
            transform.DOKill();
            transform.DOScale(scale, _uiConfig.PressDuration).SetLink(gameObject);
        }
    }
}
