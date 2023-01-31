namespace Leo.Subtypes.Flows
{
    using Leo.Subtypes.Settings;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// IFlow interface
    /// </summary>
    public interface IFlow
    {
        /// <summary>
        /// Gets the library settings
        /// </summary>
        LibrarySettings LibrarySettings { get; }

        /// <summary>
        /// Trigger before the flow is triggerd 
        /// </summary>
        public event EventHandler<FlowEventArgument> BeforeTrigger;

        /// <summary>
        /// Trigger after the flow is triggerd
        /// </summary>
        public event EventHandler<FlowEventArgument> AfterTrigger;


        /// <summary>
        /// Trigger when an error occured
        /// </summary>
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// Flow name that should be unique accross GEP eco system
        /// </summary>
        public string FlowId { get; }

   

        /// <summary>
        /// Trigger the flow 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        FlowResult Trigger(TriggerParameter arg);

        /// <summary>
        /// Trigger the flow async
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        Task<FlowResult> TriggerAsync(TriggerParameter arg);

        /// <summary>
        /// Execute the flow Template which executes Pre and Post events
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        FlowResult Execute(TriggerParameter arg);

        /// <summary>
        /// Execute the flow Template async which executes Pre and Post events
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        Task<FlowResult> ExecuteAsync(TriggerParameter arg);

    }
}