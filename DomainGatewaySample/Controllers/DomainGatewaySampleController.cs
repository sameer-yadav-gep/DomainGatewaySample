using Leo.Subtypes.Flows;
using Microsoft.AspNetCore.Mvc;

namespace DomainGatewaySample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DomainGatewaySampleController : ControllerBase
    {
        
        private readonly ILogger<DomainGatewaySampleController> _logger;
        private readonly IFlowRegistry _flowRegistry;

        public DomainGatewaySampleController(ILogger<DomainGatewaySampleController> logger, IFlowRegistry flowRegistery)
        {
            _logger = logger;
            _flowRegistry = flowRegistery;
        }

        [HttpPost("Submit")]
        public ActionResult<FlowResult> Submit(FlowTriggerCriteria flowTriggerCriteria)
        {

            return Ok(_flowRegistry.ExecuteFlow(flowTriggerCriteria, 
                new TriggerParameter() { Payload = "test"}));
        }

        [HttpPost("Reject")]
        public ActionResult<FlowResult> Reject(FlowTriggerCriteria flowTriggerCriteria)
        {

            return Ok(_flowRegistry.ExecuteFlow(flowTriggerCriteria,
                new TriggerParameter() { Payload = "test" }));
        }
    }
}