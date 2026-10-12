using Cysharp.Threading.Tasks;
using Game.Core.Services;
using Game.SceneSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameplaySystem
{
    internal class GameplayHudView : MonoBehaviour
    {
        [SerializeField] private Button _menuButton;

        private void OnEnable()
        {
            _menuButton.onClick.AddListener(OnMenuClicked);
        }

        private void OnDisable()
        {
            _menuButton.onClick.RemoveListener(OnMenuClicked);
        }

        private void OnMenuClicked()
        {
            _menuButton.interactable = false;
            ServiceLocator.Get<ISceneLoader>().LoadMainAsync().Forget();
        }
    }
}
