namespace Leo.Subtypes.Flows
{
    using System;
 
    /// <summary>
    /// Flow Result sturct 
    /// </summary>
    public class FlowResult
    {
        public FlowResult(IFlow flow)
        {
            if (flow != null)
            {
                this.FlowId = flow.FlowId;
            }
        }

        /// <summary>
        /// Flow Name
        /// </summary>
        public string FlowId { get; private set; }



        /// <summary>
        /// Flow Status
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Excpetion if exists
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Results
        /// </summary>
        public dynamic Result { get; set; }

        /// <summary>
        /// Wether is Empty Result
        /// </summary>
        public bool IsEmpty { get; set; }

        /// <summary>
        /// Create an empty result
        /// </summary>
        public static FlowResult Empty
        {
            get
            {
                return new FlowResult(null)
                {
                    IsEmpty = true
                };
            }
        }
    }
}
