using Leo.Subtypes.Settings;
using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Text;

namespace Leo.Subtypes
{
    public class LeoAssemblyLoadContext : AssemblyLoadContext
    {
        public LeoAssemblyLoadContext(string name) : base(name, true)
        {

        }
        public LibrarySettings LibrarySettings { get; set; }
    }
}
