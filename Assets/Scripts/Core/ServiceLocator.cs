using System;
using System.Collections.Generic;

namespace SlopJam.Core
{
    /// <summary>
    /// Ultra-lightweight service locator for early prototypes.
    /// Avoids committing to a DI framework before requirements stabilize.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static void Register<T>(T service) where T : class
        {
            var key = typeof(T);
            Services[key] = service ?? throw new ArgumentNullException(nameof(service));
        }

        public static bool TryResolve<T>(out T service) where T : class
        {
            if (Services.TryGetValue(typeof(T), out var obj) && obj is T typed)
            {
                service = typed;
                return true;
            }

            service = null;
            return false;
        }

        public static T Resolve<T>() where T : class
        {
            if (TryResolve(out T service))
            {
                return service;
            }

            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.");
        }
    }
}

