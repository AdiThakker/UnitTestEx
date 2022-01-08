﻿// Copyright (c) Avanade. Licensed under the MIT License. See https://github.com/Avanade/UnitTestEx

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace UnitTestEx
{
    /// <summary>
    /// Provides extension methods to support testing.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Removes all items from the <see cref="IServiceCollection"/> for the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The service <see cref="Type"/>.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns><c>true</c> if item was successfully removed; otherwise, <c>false</c>. Also returns <c>false</c> if item was not found.</returns>
        public static bool Remove<TService>(this IServiceCollection services) where TService : class
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
            return descriptor != null && services.Remove(descriptor);
        }

        /// <summary>
        /// Replaces (where existing), or adds, a singleton service <paramref name="instance"/>. 
        /// </summary>
        /// <typeparam name="TService">The service <see cref="Type"/>.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="instance">The instance value.</param>
        /// <remarks>The <see cref="IServiceCollection"/> to support fluent-style method-chaining.</remarks>
        public static IServiceCollection ReplaceSingleton<TService>(this IServiceCollection services, TService instance) where TService : class
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.Remove<TService>();
            return services.AddSingleton<TService>(instance);
        }

        /// <summary>
        /// Replaces (where existing), or adds, a scoped service <paramref name="instance"/>. 
        /// </summary>
        /// <typeparam name="TService">The service <see cref="Type"/>.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="instance">The instance value.</param>
        /// <remarks>The <see cref="IServiceCollection"/> to support fluent-style method-chaining.</remarks>
        public static IServiceCollection ReplaceScoped<TService>(this IServiceCollection services, TService instance) where TService : class
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.Remove<TService>();
            return services.AddScoped(_ => instance);
        }

        /// <summary>
        /// Replaces (where existing), or adds, a transient service <paramref name="instance"/>. 
        /// </summary>
        /// <typeparam name="TService">The service <see cref="Type"/>.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="instance">The instance value.</param>
        /// <remarks>The <see cref="IServiceCollection"/> to support fluent-style method-chaining.</remarks>
        public static IServiceCollection ReplaceTransient<TService>(this IServiceCollection services, TService instance) where TService : class
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.Remove<TService>();
            return services.AddTransient(_ => instance);
        }

        /// <summary>
        /// Create an instance of <see cref="Type"/> <typeparamref name="T"/> using Dependency Injection (DI).
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to instantiate.</typeparam>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
        /// <returns>A reference to the newly created object.</returns>
        public static T CreateInstance<T>(this IServiceProvider serviceProvider) where T : class
        {
            var type = typeof(T);
            var ctor = type.GetConstructors().FirstOrDefault();
            if (ctor == null)
                return (T)(Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Unable to instantiate Type '{type.Name}'"));

            // Simulate dependency injection for each parameter.
            var pis = ctor.GetParameters();
            var args = new object[pis.Length];
            for (int i = 0; i < pis.Length; i++)
            {
                args[i] = serviceProvider.GetService(pis[i].ParameterType);
            }

            return (T)(Activator.CreateInstance(type, args) ?? throw new InvalidOperationException($"Unable to instantiate Type '{type.Name}'"));
        }
    }
}