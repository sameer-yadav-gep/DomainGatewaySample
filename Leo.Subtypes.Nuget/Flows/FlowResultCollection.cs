namespace Leo.Subtypes.Flows
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Flow Result Collection
    /// </summary>
    public class FlowResultCollection : List<FlowResult>
    {
        /// <summary>
        /// Create a new instance of flow collection
        /// </summary>
        public FlowResultCollection()
        {

        }

        /// <summary>
        /// Create a new instance of flow
        /// </summary>
        /// <param name="capacity"></param>
        public FlowResultCollection(int capacity) : base(capacity)
        {

        }

        /// <summary>
        /// Overall status of all the result
        /// </summary>
        public bool OverallStatus
        {
            get
            {
                return this.TrueForAll(r => r.Status);
            }
        }
    }
}
