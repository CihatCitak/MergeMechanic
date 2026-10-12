using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Core.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PopupSystem
{
    internal class PopupService : BaseService, IPopupService
    {
        [SerializeField] private PopupConfig _config;
        [SerializeField] private BasePopupView[] _prefabs;
        [SerializeField] private RectTransform _root;
        [SerializeField] private Image _dimmer;

        private readonly List<BasePopupView> _instances = new();
        private readonly List<BasePopupView> _stack = new();

        private BasePopupView Top => _stack[_stack.Count - 1];

        protected override void BindServices()
        {
            ServiceLocator.Bind<IPopupService>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IPopupService>(this);
        }

        public T Show<T>() where T : BasePopupView
        {
            var popup = GetOrCreate<T>();
            if (_stack.Contains(popup))
                return popup;

            if (_stack.Count > 0)
                Top.Close(_config);

            _stack.Add(popup);
            _dimmer.transform.SetAsLastSibling();
            FadeDimmer(_config.DimAlpha, _config.OpenDuration);
            popup.Open(_config);
            return popup;
        }

        public void Close()
        {
            if (_stack.Count == 0)
                return;

            var popup = Top;
            _stack.RemoveAt(_stack.Count - 1);
            popup.Close(_config);

            if (_stack.Count == 0)
            {
                FadeDimmer(0f, _config.CloseDuration);
                return;
            }

            _dimmer.transform.SetAsLastSibling();
            Top.Open(_config);
        }

        public void CloseAll()
        {
            if (_stack.Count == 0)
                return;

            Top.Close(_config);
            _stack.Clear();
            FadeDimmer(0f, _config.CloseDuration);
        }

        private T GetOrCreate<T>() where T : BasePopupView
        {
            foreach (var instance in _instances)
            {
                if (instance is T existing)
                    return existing;
            }

            foreach (var prefab in _prefabs)
            {
                if (prefab is not T)
                    continue;

                var created = (T)Instantiate(prefab, _root);
                _instances.Add(created);
                return created;
            }

            throw new InvalidOperationException($"[PopupService] {typeof(T).Name} is not in the popup list.");
        }

        private void FadeDimmer(float alpha, float duration)
        {
            _dimmer.DOKill();
            _dimmer.raycastTarget = alpha > 0f;
            DOTween.To(() => _dimmer.color.a, SetDimmerAlpha, alpha, duration)
                .SetTarget(_dimmer)
                .SetLink(_dimmer.gameObject);
        }

        private void SetDimmerAlpha(float alpha)
        {
            var color = _dimmer.color;
            color.a = alpha;
            _dimmer.color = color;
        }
    }
}
