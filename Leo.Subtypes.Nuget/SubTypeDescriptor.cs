namespace Leo.Subtypes
{
    using System;
    
    /// <summary>
    /// Subtype attribute
    /// </summary>
    public abstract class SubTypeDescriptor
    {
        /// <summary>
        /// Name of the subtype
        /// </summary>
        public abstract string Name { get;  }

        /// <summary>
        /// Version
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// AppId
        /// </summary>
        public abstract string AppId { get;  }

        /// <summary>
        /// Layer
        /// </summary>
        public abstract string Layer { get;  }

      
    }
}
