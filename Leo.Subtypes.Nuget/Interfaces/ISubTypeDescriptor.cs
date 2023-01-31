namespace Leo.Subtypes
{
    using System;
    
    /// <summary>
    /// Subtype attribute
    /// </summary>
    public interface ISubTypeDescriptor
    {
        /// <summary>
        /// Name of the subtype
        /// </summary>
        string Name { get;  }

        /// <summary>
        /// Version
        /// </summary>
        string Version { get; }

        /// <summary>
        /// AppId
        /// </summary>
        string AppId { get;  }

        /// <summary>
        /// Layer
        /// </summary>
        string Layer { get;  }
    }
}
