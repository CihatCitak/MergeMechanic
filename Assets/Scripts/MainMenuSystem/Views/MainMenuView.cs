using Cysharp.Threading.Tasks;
using Game.Core.Services;
using Game.SceneSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenuSystem
{
    internal class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button _playButton;

        private void OnEnable()
        {
            _playButton.onClick.AddListener(OnPlayClicked);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(OnPlayClicked);
        }

        private void OnPlayClicked()
        {
            _playButton.interactable = false;
            ServiceLocator.Get<ISceneLoader>().LoadGameplayAsync().Forget();
        }
    }
}
