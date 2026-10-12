using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Common
{
    internal class ToggleView : MonoBehaviour
    {
        [SerializeField] private UIConfig _config;
        [SerializeField] private Button _button;
        [SerializeField] private RectTransform _knob;
        [SerializeField] private GameObject _onKnob;
        [SerializeField] private GameObject _offKnob;
        [SerializeField] private float _knobTravel = 60f;

        private bool _isOn;
        private Action<bool> _changed;

        private float KnobX => _isOn ? _knobTravel : -_knobTravel;

        private void Awake()
        {
            _button.onClick.AddListener(Toggle);
        }

        public void Bind(bool isOn, Action<bool> changed)
        {
            _isOn = isOn;
            _changed = changed;
            ShowState();
            _knob.DOKill();
            var position = _knob.localPosition;
            position.x = KnobX;
            _knob.localPosition = position;
        }

        private void Toggle()
        {
            _isOn = !_isOn;
            ShowState();
            _knob.DOKill();
            _knob.DOLocalMoveX(KnobX, _config.ToggleDuration).SetLink(gameObject);
            _changed(_isOn);
        }

        private void ShowState()
        {
            _onKnob.SetActive(_isOn);
            _offKnob.SetActive(!_isOn);
        }
    }
}
