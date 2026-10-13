using Game.Core.Services;
using UnityEngine;

namespace Game.UI.Toast
{
    internal class ToastService : BaseService, IToastService
    {
        [SerializeField] private ToastView _view;

        protected override void BindServices()
        {
            ServiceLocator.Bind<IToastService>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IToastService>(this);
        }

        public void Show(string message)
        {
            _view.Show(message);
        }
    }
}
