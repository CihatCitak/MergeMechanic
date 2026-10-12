using System;
using UnityEngine;

namespace Game.Tests
{
    internal static class LogUtility
    {
        public static void WithoutLogs(Action action)
        {
            Debug.unityLogger.logEnabled = false;
            try
            {
                action();
            }
            finally
            {
                Debug.unityLogger.logEnabled = true;
            }
        }
    }
}
