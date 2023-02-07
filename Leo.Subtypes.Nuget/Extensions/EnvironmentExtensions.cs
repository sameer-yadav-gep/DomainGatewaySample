namespace Leo.Subtypes.Extensions
{
    using System;
    using System.IO;

    // Used by core | Before executing a flow this uttility helps to download binaries from blob into the consumer
    public static class EnvironmentExtensions
    {
        public static string BinaryDownloadPath()
        {
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine(AppDomain.CurrentDomain.RelativeSearchPath);
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.RelativeSearchPath ?? "");
        }

        internal static void ClearBinaryDownloadDirectory()
        {
            try
            {
                // Do not clear the directory.... for now
                //if (Directory.Exists(Path.Combine(EnvironmentExtensions.BinaryDownloadPath(), "subTypes")))
                //    Directory.Delete(Path.Combine(EnvironmentExtensions.BinaryDownloadPath(), "subTypes"), true);

                Directory.CreateDirectory(Path.Combine(EnvironmentExtensions.BinaryDownloadPath(), "subTypes"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
