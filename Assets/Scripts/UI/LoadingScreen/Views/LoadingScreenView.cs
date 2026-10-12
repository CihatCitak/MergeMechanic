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
            EventBus.Subscribe<SceneLoadCompletedEvent>(OnSceneLoadCompleted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<SceneLoadStartedEvent>(OnSceneLoadStarted);
            EventBus.Unsubscribe<SceneLoadCompletedEvent>(OnSceneLoadCompleted);
        }

        private void OnSceneLoadStarted(SceneLoadStartedEvent e)
        {
            _root.SetActive(true);
        }

        private void OnSceneLoadCompleted(SceneLoadCompletedEvent e)
        {
            _root.SetActive(false);
        }
    }
}
