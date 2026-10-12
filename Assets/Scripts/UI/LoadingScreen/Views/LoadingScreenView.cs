using Game.Core.Events;
using Game.Events;
using UnityEngine;

namespace Game.UI.LoadingScreen
{
    internal class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<SceneLoadStartedEvent>(OnSceneLoadStarted);
            EventBus.Subscribe<MainMenuLoadedEvent>(OnMainMenuLoaded);
            EventBus.Subscribe<LevelReadyEvent>(OnLevelReady);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<SceneLoadStartedEvent>(OnSceneLoadStarted);
            EventBus.Unsubscribe<MainMenuLoadedEvent>(OnMainMenuLoaded);
            EventBus.Unsubscribe<LevelReadyEvent>(OnLevelReady);
        }

        private void OnSceneLoadStarted(SceneLoadStartedEvent e)
        {
            _root.SetActive(true);
        }

        private void OnMainMenuLoaded(MainMenuLoadedEvent e)
        {
            _root.SetActive(false);
        }

        private void OnLevelReady(LevelReadyEvent e)
        {
            _root.SetActive(false);
        }
    }
}
