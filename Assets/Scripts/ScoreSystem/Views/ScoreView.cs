using DG.Tweening;
using Game.Core.Services;
using TMPro;
using UnityEngine;

namespace Game.ScoreSystem
{
    internal class ScoreView : MonoBehaviour
    {
        [SerializeField] private ScoreConfig _scoreConfig;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private RectTransform _icon;

        private int _shownScore;

        public RectTransform Icon => _icon;

        private void Start()
        {
            _shownScore = ServiceLocator.Get<IScore>().Current;
            _scoreText.SetText("{0}", _shownScore);
        }

        public void Add(int score)
        {
            _shownScore += score;
            _scoreText.SetText("{0}", _shownScore);
            _icon.DOKill(true);
            _icon.DOPunchScale(Vector3.one * _scoreConfig.IconPunch, _scoreConfig.IconPunchDuration, 1, 0f).SetLink(gameObject);
        }
    }
}
