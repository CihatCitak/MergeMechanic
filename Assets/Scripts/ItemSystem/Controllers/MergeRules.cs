namespace Game.ItemSystem
{
    internal static class MergeRules
    {
        public static MergeResult Evaluate(ItemChainData draggedChain, int draggedLevel, ItemChainData targetChain, int targetLevel)
        {
            if (draggedChain != targetChain)
                return MergeResult.DifferentChain;
            if (draggedLevel != targetLevel)
                return MergeResult.DifferentLevel;
            if (draggedLevel >= draggedChain.Levels.Count)
                return MergeResult.MaxLevel;

            return MergeResult.Merge;
        }
    }
}
