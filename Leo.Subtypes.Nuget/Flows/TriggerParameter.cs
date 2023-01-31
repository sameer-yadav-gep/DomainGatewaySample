namespace Leo.Subtypes.Flows
{
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    
    /// <summary>
    /// Trigger Parameter
    /// </summary>
    public class TriggerParameter
    {
        /// <summary>
        /// Payload as JSON
        /// </summary>
        public JToken Payload { get; set; }


        /// <summary>
        /// Dictionary of request header
        /// </summary>
        public IDictionary<string, object> requestHeaders { get; set; }
    }
}
