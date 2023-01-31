namespace TcsSamplePackage
{
    public class TcsConfiguration : ITcsConfiguration
    {
        public string ExecuteTcs(string param)
        {
            return $"TCS Sample NuGet package executed with argument : {param}";
        }
    }
}