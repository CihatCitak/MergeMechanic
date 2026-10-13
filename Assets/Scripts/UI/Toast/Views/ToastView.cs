using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.UI.Toast
{
    internal class ToastView : MonoBehaviour
    {
        [SerializeField] private ToastConfig _toastConfig;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TMP_Text _message;

        public void Show(string message)
        {
            _message.text = message;
            _group.DOKill();
            DOTween.Sequence()
                .Append(_group.DOFade(1f, _toastConfig.FadeDuration))
                .AppendInterval(_toastConfig.VisibleDuration)
                .Append(_group.DOFade(0f, _toastConfig.FadeDuration))
                .SetTarget(_group)
                .SetLink(gameObject);
        }
    }
}
