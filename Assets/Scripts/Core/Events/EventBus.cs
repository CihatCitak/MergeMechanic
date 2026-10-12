using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Events
{
    public static class EventBus
    {
        public static void Subscribe<T>(Action<T> handler) where T : struct, IEvent
        {
            if (handler != null)
                Handlers<T>.List.Add(handler);
        }

        public static void Unsubscribe<T>(Action<T> handler) where T : struct, IEvent
        {
            if (handler != null)
                Handlers<T>.List.Remove(handler);
        }

        public static void Fire<T>(T e) where T : struct, IEvent
        {
            var list = Handlers<T>.List;

            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (i >= list.Count)
                    continue;

                try
                {
                    list[i](e);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        private static class Handlers<T> where T : struct, IEvent
        {
            public static readonly List<Action<T>> List = new();
        }
    }
}
