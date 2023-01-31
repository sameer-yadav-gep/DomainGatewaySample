namespace Leo.Subtypes.Settings
{
    /// <summary>
    /// Subtype setting wrapper 
    /// </summary>
    public class SubTypeSettings : ISubTypeSettings
    {
        /// <summary>
        /// Blob information
        /// </summary>
        public SubTypeBlobSettings Blob { get; set; }

        /// <summary>
        /// Local IO libs
        /// </summary>
        public SubTypeIOSettings IO { get; set; }

        /// <summary>
        /// Auto discovery
        /// </summary>

        public AutoDiscoverySettings AutoDiscovery { get; set; }
    }

    /// <summary>
    /// Auto discover settings
    /// </summary>
    public class AutoDiscoverySettings
    {
        /// <summary>
        /// Wether auto discovery is enabled or disabled. Default is false
        /// </summary>
        public bool Status { get; set; }


        /// <summary>
        /// Wether lazy loaded moudle is enabled or disabled. Default false
        /// </summary>
        public bool LazyLoaded { get; set; }


        /// <summary>
        /// Source can be blob or other lib repo 
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Blob configuration
        /// </summary>
        public SubTypeBlobSettings Blob { get; set; }

    }

    /// <summary>
    /// Setting of the subtype that gets added into the setting.json
    /// </summary>
    public class SubTypeBlobSettings
    {
        /// <summary>
        /// The rootpath where all the dlls are located
        /// </summary>
        public string ContainerName { get; set; }

        /// <summary>
        /// Blon Connection String
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// List of the dlls that will be loaded at the runtime
        /// </summary>
        public LibrarySettings[] Libraries { get; set; }
    }

    /// <summary>
    /// Setting of the subtype that gets added into the setting.json
    /// </summary>
    public class SubTypeIOSettings
    {
        /// <summary>
        /// The rootpath where all the dlls are located
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        /// List of the dlls that will be loaded at the runtime
        /// </summary>
        public LibrarySettings[] Libraries { get; set; }
    }


    /// <summary>
    /// Subtype setting info
    /// </summary>
    public class LibrarySettings
    {
        /// <summary>
        /// The full path of the dll if it is different that the root path
        /// </summary>
        public string PluginName { get; set; }

        /// <summary>
        /// The name of the dll with .dll extension
        /// </summary>
        public string LibraryName { get; set; }

        /// <summary>
        /// Library Version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Unique Name that will be used in the AssemblyContext
        /// </summary>
        public string AssemblyUniqueName { get; set; }

        /// <summary>
        /// Full location of the downloaded library
        /// </summary>
        public string DownloadFullName { get; set; }

        /// <summary>
        /// Full location of the dll directory
        /// </summary>
        public string DownloadDirectoryFullName { get; set; }


    }
}