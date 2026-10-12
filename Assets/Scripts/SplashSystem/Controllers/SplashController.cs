using System;
using Cysharp.Threading.Tasks;
using Game.Core.Services;
using Game.SceneSystem;
using UnityEngine;

namespace Game.SplashSystem
{
    internal class SplashController : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _duration = 1.5f;

        private void Start()
        {
            ShowSplashAsync().Forget();
        }

        private async UniTaskVoid ShowSplashAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: destroyCancellationToken);
            await ServiceLocator.Get<ISceneLoader>().LoadMainAsync();
        }
    }
}
