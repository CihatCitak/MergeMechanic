using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Common
{
    internal class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private UIConfig _config;
        [SerializeField] private Selectable _target;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_target.IsInteractable())
                ScaleTo(_config.PressScale);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ScaleTo(1f);
        }

        private void ScaleTo(float scale)
        {
            transform.DOKill();
            transform.DOScale(scale, _config.PressDuration).SetLink(gameObject);
        }
    }
}
