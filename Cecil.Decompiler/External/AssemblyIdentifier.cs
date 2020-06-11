using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JustDecompile.SmartAssembly.Attributes;

namespace Telerik.JustDecompiler.External
{
    [DoNotPruneType]
    [DoNotObfuscateType]
    public struct AssemblyIdentifier
    {
        private string path;

        public AssemblyIdentifier(string path)
        {
            this.path = path;
        }

        public string AssemblyPath
        {
            get
            {
                return path;
            }
            private set
            {
                this.path = value;
            }
        }
    }
}
