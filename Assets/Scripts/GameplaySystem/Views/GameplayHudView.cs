using Cysharp.Threading.Tasks;
using Game.Core.Services;
using Game.PopupSystem;
using Game.SceneSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameplaySystem
{
    internal class GameplayHudView : MonoBehaviour
    {
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _resetButton;

        private void OnEnable()
        {
            _menuButton.onClick.AddListener(OnMenuClicked);
            _resetButton.onClick.AddListener(OnResetClicked);
        }

        private void OnDisable()
        {
            _menuButton.onClick.RemoveListener(OnMenuClicked);
            _resetButton.onClick.RemoveListener(OnResetClicked);
        }

        private void OnMenuClicked()
        {
            _menuButton.interactable = false;
            ServiceLocator.Get<ISceneLoader>().LoadMainAsync().Forget();
        }

        private void OnResetClicked()
        {
            ServiceLocator.Get<IPopupService>().Show<ResetPopupView>();
        }
    }
}
