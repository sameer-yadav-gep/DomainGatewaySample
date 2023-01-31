namespace PusherSamplePackage
{
    public class PusherSample : IPusherSample
    {
        public string ExecutePusher(string param)
        {
            return $"Pusher Sample NuGet package executed with argument : {param}";
        }
    }
}