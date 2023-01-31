using System;
using System.Collections.Generic;
using System.Text;

namespace Leo.Subtypes.Flows
{
    public class FlowEventArgument : EventArgs
    {
        public TriggerParameter TriggerParams { get; set; }

        public string FlowName { get; set; }
    }
}
