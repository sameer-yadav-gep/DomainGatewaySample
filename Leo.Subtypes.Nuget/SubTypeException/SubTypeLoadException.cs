namespace Leo.Subtypes.SubTypeException
{
    using Leo.Subtypes.Flows;
    using Leo.Subtypes.Settings;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;

    [Serializable()]
    public class SubTypeLoadException : Exception
    {
        public SubTypeLoadException(LibrarySettings settings, Exception ex, Type flowType = null) : base(
            $"Unable to load subtype assembly due to " + ex.Message + " " + ex.InnerException?.Message + Environment.NewLine +
            $" Library Name:{ settings.LibraryName}  " + Environment.NewLine +
            $" Unique ID:{ settings.AssemblyUniqueName}  " + Environment.NewLine +
            $" Version: {settings.Version}. " + Environment.NewLine +
            $" Flow : {flowType?.FullName}")
        {
  
        }

        public SubTypeLoadException(string message, System.Exception inner) : base(message, inner) { }

        public SubTypeLoadException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
