namespace Leo.Subtypes.SubTypeException
{
    using Leo.Subtypes.Flows;
    using Leo.Subtypes.Settings;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;

    [Serializable()]
    public class ResolveHostDepenecyException : Exception
    {
        public ResolveHostDepenecyException(Type depenecyType) : base(
            $"Unable to load subtype assembly due to failuer to resolve depenecy " + Environment.NewLine +
            $" Depenecy: { depenecyType.FullName }  " + Environment.NewLine)
        {

        }

        public ResolveHostDepenecyException(string message, System.Exception inner) : base(message, inner) { }

        public ResolveHostDepenecyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
