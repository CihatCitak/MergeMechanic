using UnityEngine;

namespace Game.Core.Services
{
    public abstract class BaseService : MonoBehaviour, IInitializable
    {
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        protected abstract void BindServices();

        protected abstract void UnbindServices();

        protected virtual void Initialize()
        {
        }

        protected virtual void DeInitialize()
        {
        }

        protected virtual void Awake()
        {
            BindServices();
        }

        protected virtual void Start()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            Initialize();
        }

        protected virtual void OnDestroy()
        {
            if (_isInitialized)
            {
                _isInitialized = false;
                DeInitialize();
            }

            UnbindServices();
        }
    }
}
