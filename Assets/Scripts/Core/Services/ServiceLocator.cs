using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Services
{
    public sealed class ServiceLocator : MonoSingleton<ServiceLocator>
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static void Bind<T>(T service) where T : class
        {
            if (service == null)
            {
                Debug.LogError($"[ServiceLocator] Cannot bind null for {typeof(T).Name}.");
                return;
            }

            if (Services.TryGetValue(typeof(T), out var existing))
            {
                Debug.LogError($"[ServiceLocator] {typeof(T).Name} is already bound to {existing}. Bind ignored for {service}.");
                return;
            }

            Services.Add(typeof(T), service);
        }

        public static T Get<T>() where T : class
        {
            if (Services.TryGetValue(typeof(T), out var service))
                return (T)service;

            Debug.LogError($"[ServiceLocator] {typeof(T).Name} is not bound.");
            return null;
        }

        public static void Unbind<T>(T service) where T : class
        {
            if (Services.TryGetValue(typeof(T), out var existing) && ReferenceEquals(existing, service))
                Services.Remove(typeof(T));
        }
    }
}
