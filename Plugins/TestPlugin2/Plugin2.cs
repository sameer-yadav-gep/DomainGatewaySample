using Leo.Subtypes.Flows;
using Microsoft.Extensions.DependencyInjection;
using PusherSamplePackage;

namespace TestPlugin2
{
    public class Plugin2 : FlowBase

    {
        private readonly IPusherSample _pusher;
        public Plugin2(object[] args) : base(args)
        {
            _pusher = this.GetArgument<IPusherSample>(args);
            this.OnError += Plugin2_OnError;
            this.BeforeTrigger += Plugin2_BeforeTrigger;
            this.AfterTrigger += Plugin1_AfterTrigger;
        }

        private void Plugin1_AfterTrigger(object sender, FlowEventArgument e)
        {
            Console.WriteLine("Plugin2 After Trigger");
        }

        private void Plugin2_BeforeTrigger(object sender, FlowEventArgument e)
        {
            Console.WriteLine("Plugin2 Before Trigger");
        }

        private void Plugin2_OnError(object sender, Exception e)
        {
            Console.WriteLine("Plugin2 On Error");
        }

        public override string FlowId => "Plugin#2";

        public override FlowResult Trigger(TriggerParameter arg)
        {
            var result = _pusher.ExecutePusher("Plugin#2");
            return new FlowResult(this)
            {
                Status = true,
                Result = result
            };
        }
    }
}