using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rabisco.Core
{
    /// <summary>
    /// Static service locator. The only global access point for all services.
    /// Every manager registers itself here on creation (via Bootstraper).
    /// Access: ServiceLocator.Get<IGameStateService>();
    /// </summary>
    public static class ServiceLocator
    {
        #region STATIC

        private static readonly Dictionary<Type, IGameService> s_Services = new();

        #endregion


        #region PUBLIC API

        /// <summary>
        /// Registers a service instance by its interface type.
        /// Duplicate registrations are logged as warnings and the old instance is overwritten.
        /// </summary>
        /// <typeparam name="T">The interface type (e.g., IGameStateService).</typeparam>
        /// <param name="service">The service instance to register.</param>
        public static void Register<T>(T service) where T : IGameService
        {
            Type type = typeof(T);

            if (s_Services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Duplicate registration: {type.Name}. Old instance will be overwritten.");
            }

            s_Services[type] = service;
        }

        /// <summary>
        /// Retrieves a service by its interface type.
        /// Returns null if not registered — check for null before using.
        /// </summary>
        /// <typeparam name="T">The interface type to look up.</typeparam>
        /// <returns>The registered service instance, or null.</returns>
        public static T Get<T>() where T : IGameService
        {
            Type type = typeof(T);

            if (s_Services.TryGetValue(type, out IGameService service))
            {
                return (T)service;
            }

            Debug.LogError($"[ServiceLocator] Service not found: {type.Name}. Did you forget to register it in Bootstraper?");
            return default;
        }

        /// <summary>
        /// Unregisters a service by its interface type.
        /// </summary>
        /// <typeparam name="T">The interface type to unregister.</typeparam>
        public static void Unregister<T>() where T : IGameService
        {
            s_Services.Remove(typeof(T));
        }

        /// <summary>
        /// Removes all registered services. Used when returning to a clean state.
        /// </summary>
        public static void Clear()
        {
            s_Services.Clear();
            Debug.Log("[ServiceLocator] All services cleared.");
        }

        #endregion
    }
}
