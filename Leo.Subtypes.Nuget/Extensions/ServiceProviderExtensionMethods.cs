using Leo.Subtypes.SubTypeException;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leo.Subtypes.Extensions
{
    public static class ServiceProviderExtensionMethods
    {
        public static T GetHostService<T>(this IServiceProvider hostServiceProvider, IServiceScope scope)
        {
            var service = scope.ServiceProvider.GetService<T>();
            if (service == null)
                service = hostServiceProvider.GetService<T>();

            if (service != null)
                return service;
            else
                throw new ResolveHostDepenecyException(typeof(T));

        }
    }
}
