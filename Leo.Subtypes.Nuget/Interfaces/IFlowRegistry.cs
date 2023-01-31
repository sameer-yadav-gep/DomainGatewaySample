namespace Leo.Subtypes.Flows
{
    using Leo.Subtypes.Settings;
    using Leo.Subtypes.SubTypeException;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// IFlow REgistery Interface that gets injected at the runtime
    /// </summary>
    public interface IFlowRegistry : IList<IFlow>
    {
        public bool ContainsLoadError { get; }

        public List<SubTypeLoadException> LoadExecptions { get; }

        public List<LibrarySettings> DiscoverableLibraries { get; }

        IEnumerable<IFlow> FindFlows(Func<IFlow, bool> predicate);


        /// <summary>
        /// Find flow(s) by criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IFlow FindFlow(FlowTriggerCriteria criteria);

        /// <summary>
        /// Find the flow by its name
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        IFlow FindFlow(string flowId, string assemblyVersion);


        /// <summary>
        /// REgister a flow to the collection
        /// </summary>
        /// <param name="flow"></param>
        void RegisterFlow(IFlow flow);

        /// <summary>
        /// Trigger a flow by criteria and return list of result of each flow 
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        FlowResult TriggerFlow(FlowTriggerCriteria criteria, TriggerParameter arg);

        /// <summary>
        /// Trigger a flow async by criteria and return list of result of each flow 
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        Task<FlowResult> TriggerFlowAsync(FlowTriggerCriteria criteria, TriggerParameter arg);

        /// <summary>
        /// Trigger flow by it's name 
        /// </summary>
        /// <param name="flowName"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        //FlowResult TriggerFlow(string flowName, TriggerParameter arg);

        /// <summary>
        /// Execture flow template based that matchs the criteria and return the result(s) of the matched flows
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        FlowResult ExecuteFlow(FlowTriggerCriteria criteria, TriggerParameter arg);

        /// <summary>
        /// Execture flow template based that matchs the criteria and return the result(s) of the matched flows
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        Task<FlowResult> ExecuteFlowAsync(FlowTriggerCriteria criteria, TriggerParameter arg);

    }
}