using Mono.Cecil;
using System;
using System.Collections.Generic;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompile.EngineInfrastructure
{
    public class AssemblyInfo
    {
        public AssemblyInfo()
        {
            this.ModulesFrameworkVersions = new Dictionary<ModuleDefinition, FrameworkVersion>();
            this.AssemblyTypes = AssemblyTypes.Unknown;
        }

        public IDictionary<ModuleDefinition, FrameworkVersion> ModulesFrameworkVersions { get; private set; }

        public AssemblyTypes AssemblyTypes { get; set; }
    }
}