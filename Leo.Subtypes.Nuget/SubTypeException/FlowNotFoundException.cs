namespace Leo.Subtypes.SubTypeException
{
    using Leo.Subtypes.Flows;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;

    [Serializable()]
    public class FlowNotFoundException : Exception
    {
        /// <summary>
        /// Flow ID
        /// </summary>
        public string FlowId { get; set; }

        /// <summary>
        /// Running Dll version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Initalize a new instance of flow not found exeception
        /// </summary>
        /// <param name="criteria"></param>
        public FlowNotFoundException(FlowTriggerCriteria criteria) : base(
            $"Unable to find flow with the provided criteria. " + Environment.NewLine +
            $" SubType:{ criteria.SubType}  " + Environment.NewLine +
            $" Flow ID:{ criteria.FlowId }  " + Environment.NewLine +
            $" Version: {criteria.Version}. " )
        {
            this.FlowId = criteria.FlowId;
            this.Version = criteria.Version;
        }

        public FlowNotFoundException(string message, System.Exception inner) : base(message, inner) { }

        public FlowNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
