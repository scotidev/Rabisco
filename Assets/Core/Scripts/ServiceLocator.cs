using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rabisco.Core
{
    /// <summary>
    /// Global static service locator. The only global class in the project. Access is centralized here.
    /// Every manager registers its interface here on Awake and unregisters on OnDestroy.
    /// </summary>
    public static class ServiceLocator
    {
        #region STATIC

        private static readonly Dictionary<Type, object> s_Services = new();

        #endregion


        #region PUBLIC API

        /// <summary>
        /// Registers a service instance by its interface type.
        /// Call this in the manager's Awake().
        /// </summary>
        /// <typeparam name="T">The interface type (e.g., IAudioService).</typeparam>
        /// <param name="service">The manager instance implementing the interface.</param>
        public static void Register<T>(T service)
        {
            Type type = typeof(T);

            if (s_Services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Duplicate registration: {type.Name}.");
            }

            s_Services[type] = service;
        }

        /// <summary>
        /// Retrieves a service by its interface type.
        /// Returns null if not registered — check for null before using.
        /// </summary>
        /// <typeparam name="T">The interface type to look up.</typeparam>
        /// <returns>The registered service instance, or null.</returns>
        public static T Get<T>()
        {
            Type type = typeof(T);

            if (s_Services.TryGetValue(type, out object service))
            {
                return (T)service;
            }

            Debug.LogError($"[ServiceLocator] Service not found: {type.Name}. Did you forget to register it in Bootstrap?");

            return default;
        }

        /// <summary>
        /// Unregisters a service by its interface type.
        /// Call this in the manager's OnDestroy().
        /// </summary>
        /// <typeparam name="T">The interface type to unregister.</typeparam>
        public static void Unregister<T>()
        {
            Type type = typeof(T);

            if (s_Services.Remove(type))
            {
                Debug.Log($"[ServiceLocator] Unregistered: {type.Name}");
            }
        }

        /// <summary>
        /// Removes all registered services. Used when returning to Bootstrap scene.
        /// </summary>
        public static void Clear()
        {
            s_Services.Clear();

            Debug.Log("[ServiceLocator] All services cleared.");
        }

        #endregion
    }
}