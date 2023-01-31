namespace Leo.Subtypes.Flows
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class FlowTriggerCriteria
    {
        /// <summary>
        /// Flow ID
        /// </summary>
        public string FlowId { get; set; }

        /// <summary>
        /// Flow Version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Subtype name
        /// </summary>
        public string SubType { get; set; }


        /// <summary>
        /// Check if the criteria matches
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool Matches(IFlow flow)
        {
            if (flow == null)
                return false;

            if (!string.IsNullOrEmpty(this.FlowId) && !string.IsNullOrEmpty(this.Version))
            {
                return
                 (!string.IsNullOrEmpty(flow.FlowId) && this.FlowId.Equals(flow.FlowId, StringComparison.OrdinalIgnoreCase)) &&
                 (!string.IsNullOrEmpty(flow.LibrarySettings.Version) && this.Version.Equals(flow.LibrarySettings.Version, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(this.FlowId) && string.IsNullOrEmpty(this.Version))
            {
                return (!string.IsNullOrEmpty(flow.FlowId) && this.FlowId.Equals(flow.FlowId, StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }
    }
}