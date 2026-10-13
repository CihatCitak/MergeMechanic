using System.Collections.Generic;
using Game.Core.Services;
using Game.Core.Utilities;

namespace Game.PlayerSystem
{
    internal class PlayerDataStore : BaseService, IPlayerProgress
    {
        internal const string FileName = "player.json";

        private PlayerData _data;

        public int Score => _data.Score;
        public IReadOnlyList<BoardItemData> Board => _data.Board;

        protected override void BindServices()
        {
            ServiceLocator.Bind<IPlayerProgress>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IPlayerProgress>(this);
        }

        protected override void Initialize()
        {
            _data = new PlayerData();
            if (!JsonFileUtility.Load(FileName, _data))
                _data = new PlayerData();
        }

        public void SaveScore(int score)
        {
            _data.Score = score;
            Save();
        }

        public void SaveBoard(IEnumerable<BoardItemData> board)
        {
            _data.Board.Clear();
            _data.Board.AddRange(board);
            Save();
        }

        public void Reset()
        {
            _data = new PlayerData();
            JsonFileUtility.Delete(FileName);
        }

        private void Save()
        {
            JsonFileUtility.Save(FileName, _data);
        }
    }
}
