// No longer being used.
namespace Leo.Subtypes.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// This static class will have extension methods for IServiceCollection dependency injection interface 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        #region Nested Types
        /// <summary>
        /// Nested IServiceMetadata Classs
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TMetadata"></typeparam>
        private interface IServiceMetadata<TService, out TMetadata>
        {
            #region Properties

            /// <summary>
            /// Cashed Implmentation Facorty
            /// </summary>
            Func<IServiceProvider, TService> CachingImplementationFactory { get; }

            /// <summary>
            /// Metadata
            /// </summary>
            TMetadata Metadata { get; }
            #endregion
        }

        /// <summary>
        /// Nested IServiceMetadata Classs
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <typeparam name="TMetadata"></typeparam>
        private interface IServiceMetadata<TService, TImplementation, out TMetadata> : IServiceMetadata<TService, TMetadata> where TService : class
                                                                                                                             where TImplementation : class, TService
        {
            #region Properties

            /// <summary>
            /// Caching Implementation Facorty
            /// </summary>
            new Func<IServiceProvider, TImplementation> CachingImplementationFactory { get; }
            #endregion
        }

        private class ServiceMetadata<TService, TImplementation, TMetadata> : IServiceMetadata<TService, TImplementation, TMetadata> where TService : class
                                                                                                                                     where TImplementation : class, TService
        {
            #region Fields
            /// <summary>
            /// Cached Implementation Instance
            /// </summary>
            private TImplementation _cachedImplementation;
            #endregion

            #region Properties

            /// <summary>
            /// Service Metadata delegate
            /// </summary>
            Func<IServiceProvider, TService> IServiceMetadata<TService, TMetadata>.CachingImplementationFactory => CachingImplementationFactory;

            /// <summary>
            /// Caching Implmentation Metadata delegate
            /// </summary>
            public Func<IServiceProvider, TImplementation> CachingImplementationFactory { get; }

            /// <summary>
            /// Metadata
            /// </summary>
            public TMetadata Metadata { get; }
            #endregion

            /// <summary>
            /// Create new service metadata instance 
            /// </summary>
            /// <param name="metadata"></param>
            /// <param name="implementationFactory"></param>
            public ServiceMetadata(TMetadata metadata, Func<IServiceProvider, TImplementation> implementationFactory)
            {
                if (implementationFactory == null)
                    throw new ArgumentNullException(nameof(implementationFactory));
                Metadata = metadata;
                CachingImplementationFactory = s => _cachedImplementation ?? (_cachedImplementation = implementationFactory(s));
            }
        }
        #endregion

        #region Static Methods

        /// <summary>
        /// Add Scope Service to IOC
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TMetadata"></typeparam>
        /// <param name="services"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static IServiceCollection AddScoped<TService, TMetadata>(this IServiceCollection services, TMetadata metadata)
            where TService : class
        {
            return services.AddScoped<TService, TService, TMetadata>(metadata);
        }

        /// <summary>
        ///  Add Scope Service to IOC
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="services"></param>
        /// <param name="implementation"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services, TImplementation implementation, string context)
            where TService : class
            where TImplementation : class, TService
        {
            return services.AddScoped<TService, TImplementation, string>(implementation, context);
        }

        /// <summary>
        ///  Add Scope Service to IOC
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="services"></param>
        /// <param name="implementationFactory"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory, string context) where TService : class
                                                                                                                                                                                               where TImplementation : class, TService
        {
            return services.AddScoped<TService, TImplementation, string>(implementationFactory, context);
        }

        /// <summary>
        ///  Add Scope Service to IOC
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <typeparam name="TMetadata"></typeparam>
        /// <param name="services"></param>
        /// <param name="implementationFactory"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static IServiceCollection AddScoped<TService, TImplementation, TMetadata>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory, TMetadata metadata) where TService : class
                                                                                                                                                                                                              where TImplementation : class, TService
        {
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            if (implementationFactory == null)
                throw new ArgumentNullException(nameof(implementationFactory));

            return services.AddScoped<TService>(s => s.GetServices<IServiceMetadata<TService, TMetadata>>()
                                                      .OfType<IServiceMetadata<TService, TImplementation, TMetadata>>()
                                                      .First(x => Equals(x.Metadata, metadata))
                                                      .CachingImplementationFactory(s)) // This registration ensures that only one instance is created in the scope
                           .AddScoped((Func<IServiceProvider, IServiceMetadata<TService, TMetadata>>)(s => new ServiceMetadata<TService, TImplementation, TMetadata>(metadata, implementationFactory)));
        }

        /// <summary>
        ///  Add Scope Service to IOC
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <typeparam name="TMetadata"></typeparam>
        /// <param name="services"></param>
        /// <param name="implementation"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static IServiceCollection AddScoped<TService, TImplementation, TMetadata>(this IServiceCollection services, TImplementation implementation, TMetadata metadata) where TService : class
                                                                                                                                                                               where TImplementation : class, TService
        {
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            return services.AddScoped<TService>(s => s.GetServices<IServiceMetadata<TService, TMetadata>>()
                                                      .OfType<IServiceMetadata<TService, TImplementation, TMetadata>>()
                                                      .First(sm => Equals(sm.Metadata, metadata))
                                                      .CachingImplementationFactory(s))
                           .AddScoped<IServiceMetadata<TService, TMetadata>>(s => new ServiceMetadata<TService, TImplementation, TMetadata>(metadata, ss => implementation));
        }

        /// <summary>
        ///  Add Scope Service to IOC
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <typeparam name="TMetadata"></typeparam>
        /// <param name="services"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static IServiceCollection AddScoped<TService, TImplementation, TMetadata>(this IServiceCollection services, TMetadata metadata) where TService : class
                                                                                                                                               where TImplementation : class, TService
        {
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            services.TryAddTransient<TImplementation>();
            if (typeof(TService) != typeof(TImplementation))
                services.AddScoped<TService>(s => s.GetServices<IServiceMetadata<TService, TMetadata>>()
                                                   .OfType<IServiceMetadata<TService, TImplementation, TMetadata>>()
                                                   .First(sm => Equals(sm.Metadata, metadata))
                                                   .CachingImplementationFactory(s));

            return services.AddScoped<IServiceMetadata<TService, TMetadata>>(s => new ServiceMetadata<TService, TImplementation, TMetadata>(metadata, ss => ss.GetService<TImplementation>()));
        }

        /// <summary>
        ///  Add Scope Service to IOC
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IServiceCollection AddScoped<TService>(this IServiceCollection services, string context) where TService : class
        {
            return services.AddScoped<TService, string>(context);
        }

        /// <summary>
        ///  Add Scope Service to IOC
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="services"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services, string context) where TService : class
                                                                                                                                where TImplementation : class, TService
        {
            return services.AddScoped<TService, TImplementation, string>(context);
        }

        /// <summary>
        ///  Add Scope Service to IOC
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static TService GetService<TService>(this IServiceProvider serviceProvider, string context)
        {
            return context == null ? serviceProvider.GetService<TService, string>() :
                serviceProvider.GetService<TService, string>(m => m == context);
        }

        /// <summary>
        ///  Add Scope Service to IOC
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TMetadata"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TService GetService<TService, TMetadata>(this IServiceProvider serviceProvider, Func<TMetadata, bool> predicate = null)
        {
            var relevantMetadata = serviceProvider.GetServices<IServiceMetadata<TService, TMetadata>>();
            var selectedMetadata = predicate != null ? relevantMetadata.FirstOrDefault(sm => predicate(sm.Metadata)) : relevantMetadata.FirstOrDefault();
            return selectedMetadata != null ? selectedMetadata.CachingImplementationFactory(serviceProvider) : default;
        }

        /// <summary>
        ///  Add Scope Service to IOC
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TMetadata"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<TService> GetServices<TService, TMetadata>(this IServiceProvider serviceProvider, Func<TMetadata, bool> predicate = null)
        {
            var metadata = serviceProvider.GetServices<IServiceMetadata<TService, TMetadata>>();

            return (predicate != null ? metadata.Where(sm => predicate(sm.Metadata)) : metadata).Select(sm => sm.CachingImplementationFactory(serviceProvider));
        }

        #endregion
    }
}
