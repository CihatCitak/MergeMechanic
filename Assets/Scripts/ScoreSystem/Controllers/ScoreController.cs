using Game.Core.Events;
using Game.Core.Services;
using Game.Events;
using Game.PlayerSystem;

namespace Game.ScoreSystem
{
    internal class ScoreController : BaseService, IScore
    {
        public int Current => ServiceLocator.Get<IPlayerProgress>().Score;

        protected override void BindServices()
        {
            ServiceLocator.Bind<IScore>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IScore>(this);
        }

        protected override void Initialize()
        {
            EventBus.Subscribe<ItemMergedEvent>(OnItemMerged);
        }

        protected override void DeInitialize()
        {
            EventBus.Unsubscribe<ItemMergedEvent>(OnItemMerged);
        }

        private void OnItemMerged(ItemMergedEvent e)
        {
            var playerProgress = ServiceLocator.Get<IPlayerProgress>();
            playerProgress.SaveScore(playerProgress.Score + e.Score);
        }
    }
}
