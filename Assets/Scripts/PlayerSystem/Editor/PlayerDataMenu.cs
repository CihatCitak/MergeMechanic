using Game.Core.Utilities;
using UnityEditor;

namespace Game.PlayerSystem
{
    internal static class PlayerDataMenu
    {
        private const string MenuPath = "MergeMechanic/Reset Player Data";

        [MenuItem(MenuPath)]
        private static void ResetPlayerData()
        {
            if (EditorUtility.DisplayDialog("Reset Player Data", "Delete the saved score and board?", "Reset", "Cancel"))
                JsonFileUtility.Delete(PlayerDataStore.FileName);
        }

        [MenuItem(MenuPath, true)]
        private static bool CanResetPlayerData()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }
    }
}
