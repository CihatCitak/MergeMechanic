using Game.Core.Events;
using Game.Core.Services;
using Game.Events;

namespace Game.ScoreSystem
{
    internal class ScoreController : BaseService, IScore
    {
        public int Current { get; private set; }

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
            Current += e.Score;
        }
    }
}
