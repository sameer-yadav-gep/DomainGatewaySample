using Leo.Subtypes.Extensions;
using Leo.Subtypes.Flows;
using Microsoft.Extensions.DependencyInjection;
using System;
using TcsSamplePackage;

namespace TestPlugin1
{
    public class Plugin1 : FlowBase
    {
        private readonly ITcsConfiguration _tcsConfig;
        public Plugin1(object[] args) : base(args)
        {
            //_tcsConfig = this.GetArgument<ITcsConfiguration>(args);
            var scope = base.HostServiceProvider.CreateScope();
            _tcsConfig = base.HostServiceProvider.GetHostService<ITcsConfiguration>(scope);

            this.OnError += Plugin1_OnError;
            this.BeforeTrigger += Plugin1_BeforeTrigger;
            this.AfterTrigger += Plugin1_AfterTrigger;
        }

        private void Plugin1_AfterTrigger(object sender, FlowEventArgument e)
        {
            Console.WriteLine("Plugin1 After Trigger");
        }

        private void Plugin1_BeforeTrigger(object sender, FlowEventArgument e)
        {
            Console.WriteLine("Plugin1 Before Trigger");
        }

        private void Plugin1_OnError(object sender, Exception e)
        {
            Console.WriteLine("Plugin1 On Error");
        }

        public override string FlowId => "Plugin#1";

        public override FlowResult Trigger(TriggerParameter arg)
        {
            var result = _tcsConfig.ExecuteTcs("Plugin#1");
            return new FlowResult(this)
            {
                Status = true,
                Result = result
            };
        }
    }
}