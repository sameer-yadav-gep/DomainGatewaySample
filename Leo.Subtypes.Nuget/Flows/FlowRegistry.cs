namespace Leo.Subtypes.Flows
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Reflection;
    using Leo.Subtypes.Settings;
    using System.IO;
    using Leo.Subtypes.Extensions;
    using Azure.Storage.Blobs;
    using Leo.Subtypes.SubTypeException;
    using Leo.Subtypes.AssemblyLoader;

    /// <summary>
    /// Flow Registery Implementaton 
    /// </summary>
    public class FlowRegistry : List<IFlow>, IFlowRegistry
    {
        private const string subTypeDllPrefix = "leo.subtype";

        private List<SubTypeLoadException> loadException = new List<SubTypeLoadException>();

        /// <summary>
        /// Subtype setting that is inialized in the host.
        /// </summary>
        private readonly ISubTypeSettings _setting;

        /// <summary>
        /// Host service provider
        /// </summary>

        private readonly IServiceProvider _hostServiceProvider;

        /// <summary>
        /// List of all the loaded assembly 
        /// </summary>

        private List<LeoAssemblyLoadContext> loadedAssemblyList = new List<LeoAssemblyLoadContext>();

        /// <summary>
        /// List of all discoverable libraries from the dll repo
        /// </summary>
        private List<LibrarySettings> discoverableLibraries = new List<LibrarySettings>();

        /// <summary>
        /// List of all discoverable subtype libraries from the dll repo
        /// </summary>
        private List<LibrarySettings> discoverableSubTypeLibraries = new List<LibrarySettings>();

        public bool ContainsLoadError => this.loadException.Count() != 0;

        public List<SubTypeLoadException> LoadExecptions => loadException;

        public List<LibrarySettings> DiscoverableLibraries => discoverableLibraries;


        /// <summary>
        /// Create an new instance of the reigstry. This will get intizalized from the depenecy registeration 
        /// </summary>
        /// <param name="resolver"></param>
        public FlowRegistry(ISubTypeSettings setting, IServiceProvider serviceProvider)
        {
            _setting = setting;
            _hostServiceProvider = serviceProvider;

            if (_setting.AutoDiscovery != null && _setting.AutoDiscovery.Status)
            {
                if (_setting.AutoDiscovery.Blob != null)
                {
                    var pluginContainer = new BlobContainerClient(_setting.AutoDiscovery.Blob.ConnectionString, _setting.AutoDiscovery.Blob.ContainerName);
                    var blobs = pluginContainer.GetBlobs().ToList();
                    foreach (var blob in blobs)
                    {
                        string blobName = blob.Name;
                        string[] blobNameSplitted = blobName.Split('/', StringSplitOptions.RemoveEmptyEntries);
                        if (blobNameSplitted.Length == 3)
                        {
                            LibrarySettings lib = new LibrarySettings()
                            {
                                AssemblyUniqueName = Guid.NewGuid().ToString(),
                                PluginName = blobNameSplitted[0],
                                Version = blobNameSplitted[1],
                                LibraryName = blobNameSplitted[2]
                            };

                            lib.DownloadDirectoryFullName = Path.Combine(EnvironmentExtensions.BinarayDownloadPath(), "subTypes",
                                lib.PluginName, lib.Version);
                            lib.DownloadFullName = Path.Combine(lib.DownloadDirectoryFullName, lib.LibraryName);

                            discoverableLibraries.Add(lib);
                        }
                    }

                    foreach (var lib in discoverableLibraries)
                    {
                        if (lib.LibraryName.ToLower().StartsWith(subTypeDllPrefix))
                        {
                            if (_setting.AutoDiscovery.LazyLoaded)
                            {
                                discoverableSubTypeLibraries.Add(lib);
                            }
                            else
                            {
                                LoadAssemblyFromBlob(_setting.AutoDiscovery.Blob, lib);
                            }
                        }
                    }
                }
            }

            if (_setting.IO != null)
            {
                foreach (var library in _setting.IO.Libraries)
                {
                    library.AssemblyUniqueName = Guid.NewGuid().ToString();
                    library.DownloadFullName = Path.Combine(library.DownloadDirectoryFullName ?? _setting.IO.RootPath, library.LibraryName);
                    library.DownloadDirectoryFullName = library.DownloadDirectoryFullName ?? _setting.IO.RootPath;
                    discoverableLibraries.Add(library);
                    LoadAssembly(library);
                }
            }

            if (_setting.Blob != null)
            {
                foreach (var library in _setting.Blob.Libraries)
                {
                    library.DownloadDirectoryFullName = Path.Combine(EnvironmentExtensions.BinarayDownloadPath(), "subTypes",
                        library.PluginName, library.Version);
                    library.DownloadFullName = Path.Combine(library.DownloadDirectoryFullName, library.LibraryName);

                    LoadAssemblyFromBlob(_setting.Blob, library);
                }
            }
        }


        /// <summary>
        /// Load Asssmbly from blob
        /// </summary>
        /// <param name="blobSettings"></param>
        /// <param name="lib"></param>
        private void LoadAssemblyFromBlob(SubTypeBlobSettings blobSettings, LibrarySettings lib)
        {
            if (!File.Exists(lib.DownloadFullName))
            {
                var blobClient = new BlobClient(
                    blobSettings.ConnectionString,
                    blobSettings.ContainerName,
                    $"{lib.PluginName}/{lib.Version}/{lib.LibraryName}");

                Directory.CreateDirectory(lib.DownloadDirectoryFullName);

                var result = blobClient.DownloadTo(lib.DownloadFullName);

                if (result.Status == 206 || result.Status == 200)
                {
                    try
                    {
                        var libDepenencies = discoverableLibraries
                            .Where(b =>
                            b.PluginName.Equals(lib.PluginName, StringComparison.InvariantCultureIgnoreCase)
                            && b.Version.Equals(lib.Version, StringComparison.InvariantCultureIgnoreCase)
                            && !b.LibraryName.Equals(lib.LibraryName, StringComparison.InvariantCultureIgnoreCase))
                            .ToList();

                        foreach (var libDepnency in libDepenencies)
                        {
                            var blobDepencyClient = new BlobClient(
                             blobSettings.ConnectionString,
                             blobSettings.ContainerName,
                             $"{libDepnency.PluginName}/{libDepnency.Version}/{libDepnency.LibraryName}");

                            blobDepencyClient.DownloadTo(libDepnency.DownloadFullName);
                        }
                        LoadAssembly(lib);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unable to load subtype assembly due to  {ex.Message}");
                    }
                }
            }
            else
            {
                try
                {
                    LoadAssembly(lib);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to load subtype assembly due to  {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Load the assembly to a new context
        /// </summary>
        /// <param name="library"></param>
        private void LoadAssembly(LibrarySettings library)
        {
            try
            {
                Assembly assembly = null;
                if (!this.loadedAssemblyList.Any(a => a.Name == library.AssemblyUniqueName))
                {
                    var assemblyContext = new LeoAssemblyLoadContext(library.AssemblyUniqueName);
                    assemblyContext.LibrarySettings = library;

                    // Load the actuall DLL invoice.dll
                    assembly = assemblyContext.LoadFromAssemblyPath(library.DownloadFullName);

                    // Load any refs\deps used by invoice.dll
                    var dllToLoad = Directory.GetFiles(library.DownloadDirectoryFullName, "*.dll")
                        .Where(f => !f.Equals(library.DownloadFullName, StringComparison.InvariantCultureIgnoreCase));
                    foreach (var depencydll in dllToLoad)
                    {
                        assemblyContext.LoadFromAssemblyPath(depencydll);
                    }

                    loadedAssemblyList.Add(assemblyContext);

                    var flows = (from t in assembly.GetExportedTypes()
                                 where t.GetInterfaces().Any(i => i.FullName == (typeof(IFlow).FullName))
                                 select t);

                    foreach (var f in flows)
                    {
                        try
                        {
                            var instance = Activator.CreateInstance(f, (object)(new object[2] { library, _hostServiceProvider }));
                            Console.WriteLine(instance);
                            var xx = instance is FlowBase;
                            var yy = instance is IFlow;
                            var tt = instance.GetType().BaseType;
                            var ss = typeof(FlowBase);
                            Console.WriteLine(xx);
                            Console.WriteLine(yy);
                            Console.WriteLine(tt);
                            Console.WriteLine(ss.FullName + ' ' + ss.Assembly.Location);
                            Console.WriteLine(tt.FullName + ' ' + tt.Assembly.Location);
                            this.RegisterFlow(instance as IFlow);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message + " " + ex.InnerException?.Message);
                            this.loadException.Add(new SubTypeLoadException(library, ex, f));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                loadException.Add(new SubTypeLoadException(library, ex));
                Console.WriteLine($"Unable to load subtype assembly due to  {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Unload Assembly from the context
        /// </summary>
        /// <param name="settings">Liberary Settings</param>
        /// <returns></returns>
        public bool UnLoadAssembly(LibrarySettings settings)
        {
            var assembly = this.loadedAssemblyList.FirstOrDefault(a => a.LibrarySettings.AssemblyUniqueName == settings.AssemblyUniqueName);
            assembly.Unload();
            this.loadedAssemblyList.Remove(assembly);
            this.RemoveAll(f => f.LibrarySettings.AssemblyUniqueName == settings.AssemblyUniqueName);
            return true;
        }

        /// <summary>
        /// Register a flow to the current registery
        /// </summary>
        /// <param name="flow"></param>
        public void RegisterFlow(IFlow flow)
        {
            if (flow == null)
            {
                throw new ArgumentNullException("Could not register a flow.");
            }
            if (this.FindFlow(flow.FlowId, flow.LibrarySettings.Version) == null)
                this.Add(flow);
            else
                this.ReplaceFlow(flow);
        }

        /// <summary>
        /// Replace flow if it's name exists
        /// </summary>
        /// <param name="flow"></param>
        private void ReplaceFlow(IFlow flow)
        {
            int index = this.FindIndex(f => f.FlowId.Equals(flow.FlowId, StringComparison.InvariantCultureIgnoreCase));
            if (index != -1)
                this[index] = flow;
        }

        /// <summary>
        /// Find flow by name
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public IFlow FindFlow(string flowId, string assemblyVersion)
        {
            if (!string.IsNullOrWhiteSpace(flowId) && !string.IsNullOrWhiteSpace(assemblyVersion))
            {
                return this.FirstOrDefault(f =>
                    f.FlowId.Equals(flowId, StringComparison.InvariantCultureIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(f.LibrarySettings.Version) &&
                    f.LibrarySettings.Version.Equals(assemblyVersion, StringComparison.OrdinalIgnoreCase));
            }

            return null;
        }

        /// <summary>
        /// Find the first flow that matches the criteria 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IFlow FindFlow(FlowTriggerCriteria criteria)
        {
            if (this._setting.AutoDiscovery != null && this._setting.AutoDiscovery.Status && this._setting.AutoDiscovery.LazyLoaded)
            {
                LibrarySettings library = null;

                if (!string.IsNullOrWhiteSpace(criteria.SubType) && !string.IsNullOrWhiteSpace(criteria.Version))
                {
                    library = this.discoverableSubTypeLibraries
                        .FirstOrDefault(l =>
                            l.PluginName.Equals(criteria.SubType, StringComparison.OrdinalIgnoreCase) &&
                            l.Version.Equals(criteria.Version, StringComparison.OrdinalIgnoreCase)
                         );
                }

                if (!string.IsNullOrWhiteSpace(criteria.SubType) && string.IsNullOrWhiteSpace(criteria.Version))
                {
                    library = this.discoverableSubTypeLibraries
                        .Where(l => l.PluginName.Equals(criteria.SubType, StringComparison.OrdinalIgnoreCase))
                        .OrderByDescending(l => l.Version.ConvertVersionToNumber())
                        .FirstOrDefault();
                }

                if (library == null)
                {
                    throw new FlowNotFoundException(criteria);
                }

                LoadAssemblyFromBlob(this._setting.AutoDiscovery.Blob, library);
            }
            return this
                .Where(f => criteria.Matches(f))
                .OrderByDescending(f => f.LibrarySettings.Version.ConvertVersionToNumber())
                .FirstOrDefault();
        }



        /// <summary>
        /// Trigger Flow based that matchs the criteria and return the result(s) of the matched flows
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<FlowResult> TriggerFlowAsync(FlowTriggerCriteria criteria, TriggerParameter arg)
        {
            var flow = this.FindFlow(criteria);
            if (flow == null)
                throw new FlowNotFoundException(criteria);

            return await flow.TriggerAsync(arg);


        }

        /// <summary>
        /// Trigger Flow based that matchs the criteria and return the result(s) of the matched flows
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public FlowResult TriggerFlow(FlowTriggerCriteria criteria, TriggerParameter arg)
        {
            var flow = this.FindFlow(criteria);
            if (flow == null)
                throw new FlowNotFoundException(criteria);

            return flow.Trigger(arg);

        }

        /// <summary>
        /// Execute the flow template based on the matches the criteria and return the result(s) of the matched flows
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public FlowResult ExecuteFlow(FlowTriggerCriteria criteria, TriggerParameter arg)
        {
            var flow = this.FindFlow(criteria);
            if (flow == null)
                throw new FlowNotFoundException(criteria);

            return flow.Execute(arg);
        }

        /// <summary>
        /// Execture flow template based that matchs the criteria and return the result(s) of the matched flows
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<FlowResult> ExecuteFlowAsync(FlowTriggerCriteria criteria, TriggerParameter arg)
        {
            var flow = this.FindFlow(criteria);
            if (flow == null)
                throw new FlowNotFoundException(criteria);

            return await flow.ExecuteAsync(arg);
        }

        public IEnumerable<IFlow> FindFlows(Func<IFlow, bool> predicate)
        {
            return this.Where(predicate);
        }


    }
}