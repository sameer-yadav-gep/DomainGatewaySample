namespace Leo.Subtypes.Extensions
{
    using System;
    using System.IO;

    // Used by core | Before executing a flow this uttility helps to download binaries from blob into the consumer
    public static class EnvironmentExtensions
    {
        public static string BinarayDownloadPath()
        {
            return System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                System.AppDomain.CurrentDomain.RelativeSearchPath ?? "");
        }

        internal static void ClearBinarayDownloadDirectory()
        {
            try
            {
                if (Directory.Exists(Path.Combine(EnvironmentExtensions.BinarayDownloadPath(), "subTypes")))
                    Directory.Delete(Path.Combine(EnvironmentExtensions.BinarayDownloadPath(), "subTypes"), true);

                Directory.CreateDirectory(Path.Combine(EnvironmentExtensions.BinarayDownloadPath(), "subTypes"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
