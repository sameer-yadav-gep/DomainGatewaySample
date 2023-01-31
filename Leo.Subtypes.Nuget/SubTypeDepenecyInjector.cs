namespace Leo.Subtypes.DependencyInjection
{
    using Leo.Subtypes.Extensions;
    using Leo.Subtypes.Flows;
    using Leo.Subtypes.Settings;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// SubType Depenecy Loader
    /// </summary>
    public static class SubTypeDependencyInjector
    {
        /// <summary>
        /// Load subtype dependencies
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="config"></param>
        /// <param name="configKey"></param>
        public static void RegisterSubTypeFlows(this IServiceCollection serviceCollection,
                IConfiguration config, string configKey = "SubTypeSettings")
        {
            EnvironmentExtensions.ClearBinarayDownloadDirectory();
            var configSection = config.GetSection(configKey);
            if (configSection == null)
                throw new Exception("Unable to load subtype dependencies. Missing setting elemenet in appsettings.json");
            var subtypesSettings = configSection.Get<SubTypeSettings>();
            if (subtypesSettings == null)
                throw new Exception("Unable to load subtype settings. Invalid setting elemenet in appsettings.json");

            //Register Subtype Dependencies
            serviceCollection.AddSingleton<ISubTypeSettings>(subtypesSettings);

            serviceCollection.AddScoped<IFlowRegistry, FlowRegistry>();
        }


    }
}