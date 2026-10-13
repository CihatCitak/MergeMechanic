using Cysharp.Threading.Tasks;
using Game.Core.Services;
using Game.PlayerSystem;
using Game.PopupSystem;
using Game.SceneSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameplaySystem
{
    internal class ResetPopupView : BasePopupView
    {
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        private void OnEnable()
        {
            _confirmButton.onClick.AddListener(OnConfirmClicked);
            _cancelButton.onClick.AddListener(OnCancelClicked);
        }

        private void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(OnConfirmClicked);
            _cancelButton.onClick.RemoveListener(OnCancelClicked);
        }

        private void OnConfirmClicked()
        {
            _confirmButton.interactable = false;
            ServiceLocator.Get<IPlayerProgress>().Reset();
            ServiceLocator.Get<ISceneLoader>().LoadGameplayAsync().Forget();
        }

        private void OnCancelClicked()
        {
            ServiceLocator.Get<IPopupService>().Close();
        }
    }
}
