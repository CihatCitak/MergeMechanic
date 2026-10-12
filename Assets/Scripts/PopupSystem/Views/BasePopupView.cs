using DG.Tweening;
using UnityEngine;

namespace Game.PopupSystem
{
    public abstract class BasePopupView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _panel;

        internal void Open(PopupConfig config)
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            _canvasGroup.interactable = true;
            _canvasGroup.DOKill();
            _panel.DOKill();

            _canvasGroup.alpha = 0f;
            DOTween.To(() => _canvasGroup.alpha, SetAlpha, 1f, config.OpenDuration)
                .SetTarget(_canvasGroup)
                .SetLink(gameObject);

            _panel.localScale = Vector3.one * config.StartScale;
            _panel.DOScale(1f, config.OpenDuration)
                .SetEase(config.OpenEase)
                .SetLink(gameObject);
        }

        internal void Close(PopupConfig config)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.DOKill();
            DOTween.To(() => _canvasGroup.alpha, SetAlpha, 0f, config.CloseDuration)
                .SetTarget(_canvasGroup)
                .SetLink(gameObject)
                .OnComplete(Deactivate);
        }

        private void SetAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }

        private void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
