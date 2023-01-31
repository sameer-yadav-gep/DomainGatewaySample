namespace Leo.Subtypes.Flows
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Leo.Subtypes.Settings;


    public abstract class FlowBase : IFlow
    {
        #region Properties 

        protected IServiceProvider HostServiceProvider;
      
        private LibrarySettings _librarySettings;

        private string runningAssemblyVersion = "";

        public string AssemblyVersion { get => runningAssemblyVersion; }

        public LibrarySettings LibrarySettings => _librarySettings;

        public event EventHandler<FlowEventArgument> BeforeTrigger;

        public event EventHandler<FlowEventArgument> AfterTrigger;

        public event EventHandler<Exception> OnError;

        #endregion

        #region Constructor 


        /// <summary>
        /// Inialize flow 
        /// </summary>
        /// <param name="args"></param>
        public FlowBase(object[] args)
        {
            this._librarySettings = this.GetArgument<LibrarySettings>(args);
            this.HostServiceProvider = this.GetArgument<IServiceProvider>(args);
         
            this.runningAssemblyVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();
        }
        
        #endregion

        #region Abstract Functions 

        /// <summary>
        /// Unique flow id per subtype
        /// </summary>
        public abstract string FlowId { get; }

        /// <summary>
        /// Trigger flow implementation
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public abstract FlowResult Trigger(TriggerParameter arg); 

        #endregion

        #region Facade Functions 

        /// <summary>
        /// Trigger flow implmentation in async mode
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<FlowResult> TriggerAsync(TriggerParameter arg)
        {
            return await Task<FlowResult>.Run(() => { return Trigger(arg); });
        }

        /// <summary>
        /// Execute flow implementation that wrap before and after trigger call back
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public FlowResult Execute(TriggerParameter arg)
        {
            FlowResult result = FlowResult.Empty;
            var triggerArgs = new FlowEventArgument() { TriggerParams = arg, FlowName = this.FlowId };

            try
            {
                BeforeTrigger?.Invoke(this, triggerArgs);

                result = this.Trigger(arg);

                AfterTrigger?.Invoke(this, triggerArgs);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, ex);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Execute implementation async
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<FlowResult> ExecuteAsync(TriggerParameter arg)
        {
            return await Task<FlowResult>.Run(() => { return Execute(arg); });
        } 

        #endregion

        #region Helper Functions

        /// <summary>
        /// Get Argument by type
        /// </summary>
        /// <typeparam name="T"> Target Type</typeparam>
        /// <param name="args">Array of object containing the type</param>
        /// <returns></returns>
        public T GetArgument<T>(object[] args)
        {
            foreach (var arg in args)
            {
                if (typeof(T).IsInterface)
                {
                    if (arg.GetType().GetInterfaces().Any(i => i == typeof(T)))
                        return (T)arg;
                }
                else
                {
                    if (arg.GetType() == typeof(T))
                    {
                        return (T)arg;
                    }
                }
            }
            return default(T);
        }
        #endregion
    }
}
