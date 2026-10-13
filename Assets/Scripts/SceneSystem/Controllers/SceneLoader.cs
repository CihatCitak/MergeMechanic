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
        [SerializeField] private string _mainSceneName = "MainMenu";
        [SerializeField] private string _gameplaySceneName = "Gameplay";

        protected override void BindServices()
        {
            ServiceLocator.Bind<ISceneLoader>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<ISceneLoader>(this);
        }

        public UniTask LoadMainAsync()
        {
            return LoadAsync(_mainSceneName);
        }

        public UniTask LoadGameplayAsync()
        {
            return LoadAsync(_gameplaySceneName);
        }

        private static async UniTask LoadAsync(string sceneName)
        {
            EventBus.Fire(new SceneLoadStartedEvent());
            await SceneManager.LoadSceneAsync(sceneName);
            EventBus.Fire(new SceneLoadCompletedEvent());
        }
    }
}
