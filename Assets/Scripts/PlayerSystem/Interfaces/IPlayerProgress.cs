using System.Collections.Generic;

namespace Game.PlayerSystem
{
    public interface IPlayerProgress
    {
        int Score { get; }
        IReadOnlyList<BoardItemData> Board { get; }
        void SaveScore(int score);
        void SaveBoard(IEnumerable<BoardItemData> board);
        void Reset();
    }
}
