using Cysharp.Threading.Tasks;
using Game.Core.Events;
using Game.Core.Services;
using Game.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.SceneSystem
{
    internal class SceneLoader : BaseService, ISceneLoader
    {
        [SerializeField] private string _mainSceneName = "Main";
        [SerializeField] private string _gameplaySceneName = "Gameplay";

        protected override void BindServices()
        {
            ServiceLocator.Bind<ISceneLoader>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<ISceneLoader>(this);
        }

        protected override void Initialize()
        {
            LoadMainAsync().Forget();
        }

        public async UniTask LoadMainAsync()
        {
            EventBus.Fire(new SceneLoadStartedEvent());
            await SceneManager.LoadSceneAsync(_mainSceneName);
            EventBus.Fire(new MainMenuLoadedEvent());
        }

        public async UniTask LoadGameplayAsync()
        {
            EventBus.Fire(new SceneLoadStartedEvent());
            await SceneManager.LoadSceneAsync(_gameplaySceneName);
        }
    }
}
