using TMPro;

namespace Game.UI.Common
{
    public static class TimeTextUtility
    {
        private const int SecondsPerMinute = 60;

        public static void SetMinutesSeconds(TMP_Text text, int totalSeconds)
        {
            text.SetText("{0:00}:{1:00}", totalSeconds / SecondsPerMinute, totalSeconds % SecondsPerMinute);
        }
    }
}
